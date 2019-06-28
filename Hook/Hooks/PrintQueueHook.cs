using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Threading;
using EventHook.Hooks.Library;
using Microsoft.Win32.SafeHandles;

namespace EventHook.Hooks
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct DEVMODE
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmDeviceName;
		public short dmSpecVersion;
		public short dmDriverVersion;
		public short dmSize;
		public short dmDriverExtra;
		public int dmFields;
		public short dmOrientation;
		public short dmPaperSize;
		public short dmPaperLength;
		public short dmPaperWidth;
		public short dmScale;
		public short dmCopies;
		public short dmDefaultSource;
		public short dmPrintQuality;
		public short dmColor;
		public short dmDuplex;
		public short dmYResolution;
		public short dmTTOption;
		public short dmCollate;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmFormName;
		public short dmLogPixels;
		public int dmBitsPerPel;
		public int dmPelsWidth;
		public int dmPelsHeight;
		public int dmDisplayFlags;
		public int dmDisplayFrequency;
	}
	
	public class JobDetail
	{
		public DEVMODE DevMode { get; internal set; }
	}
	
    /// <summary>
    ///  //http://www.codeproject.com/Articles/51085/Monitor-jobs-in-a-printer-queue-NET
    /// </summary>
    internal class PrintJobChangeEventArgs : EventArgs
    {
        internal PrintJobChangeEventArgs(int intJobID, string strJobName, JOBSTATUS jStatus, JobDetail detail, PrintSystemJobInfo objJobInfo)
        {
            _jobId = intJobID;
            _jobName = strJobName;
            _jobStatus = jStatus;
            _jobInfo = objJobInfo;
            _jobDetail = detail;
        }

        internal int JobId
        {
            get { return _jobId; }
        }

        internal string JobName
        {
            get { return _jobName; }
        }

        internal JOBSTATUS JobStatus
        {
            get { return _jobStatus; }
        }

        internal PrintSystemJobInfo JobInfo
        {
            get { return _jobInfo; }
        }

        private JobDetail _jobDetail;
        internal JobDetail JobDetail
        {
	        get { return _jobDetail; }
        }

        #region private variables

        private readonly int _jobId;
        private readonly string _jobName;
        private readonly JOBSTATUS _jobStatus;
        private readonly PrintSystemJobInfo _jobInfo;

        #endregion
    }

    internal delegate void PrintJobStatusChanged(object sender, PrintJobChangeEventArgs e);

    internal class PrintQueueHook
    {
        #region Constants

        private const int PRINTER_NOTIFY_OPTIONS_REFRESH = 1;

        #endregion

        #region constructor

        internal PrintQueueHook(string strSpoolName)
        {
            // Let us open the printer and get the printer handle.
            SpoolerName = strSpoolName;
        }

        #endregion

        #region Events

        internal event PrintJobStatusChanged OnJobStatusChange;

        #endregion

        #region destructor

        ~PrintQueueHook()
        {
            Stop();
        }

        #endregion

        #region StartMonitoring

        internal void Start()
        {
            OpenPrinter(SpoolerName, out _printerHandle, 0);
            if (_printerHandle != IntPtr.Zero)
            {
                //We got a valid Printer handle.  Let us register for change notification....
                _changeHandle = FindFirstPrinterChangeNotification(_printerHandle,
                    (int) PRINTER_CHANGES.PRINTER_CHANGE_ALL, 0, _notifyOptions);
                // We have successfully registered for change notification.  Let us capture the handle...
                _mrEvent.SafeWaitHandle = new SafeWaitHandle(_changeHandle, true);

                //Now, let us wait for change notification from the printer queue....
                _waitHandle = ThreadPool.RegisterWaitForSingleObject(_mrEvent, PrinterNotifyWaitCallback, _mrEvent, -1,
                    true);
            }

            _spooler = new PrintQueue(new PrintServer(), SpoolerName);
            foreach (var psi in _spooler.GetPrintJobInfoCollection())
            {
                _objJobDict[psi.JobIdentifier] = psi.Name;
            }
        }

        #endregion

        #region StopMonitoring

        internal void Stop()
        {
            try
            {
                if (_printerHandle != IntPtr.Zero)
                {
                    ClosePrinter((int)_printerHandle);
                    _printerHandle = IntPtr.Zero;
                }
            }
            catch { }
        }

		#endregion

		private JobDetail GetJobDetail(uint jobId) 
		{
			UInt32 needed = 0;
			bool result;
			
			result = GetJob(_printerHandle, jobId, 2, IntPtr.Zero, 0, out needed);
			if (Marshal.GetLastWin32Error() != 122)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Get Job 1 failure, error code=" + Marshal.GetLastWin32Error());
				Console.ForegroundColor = ConsoleColor.White;
			}
			else
			{
				Console.WriteLine("buffer size required=" + needed);
				IntPtr buffer = Marshal.AllocHGlobal((int)needed);
				result = GetJob(_printerHandle, jobId, 2, buffer, needed, out needed);
				JOB_INFO_2 jobInfo = (JOB_INFO_2)Marshal.PtrToStructure(buffer, typeof(JOB_INFO_2));
				DEVMODE dMode = (DEVMODE)Marshal.PtrToStructure(jobInfo.pDevMode, typeof(DEVMODE));
				
				return new JobDetail
				{
					DevMode = dMode
				};
//				Console.ForegroundColor = ConsoleColor.Yellow;
//				Console.WriteLine("Time now: " + DateTime.Now.ToString("hh:mm:ss"));
//				Console.ForegroundColor = ConsoleColor.White;
				//Marshal.FreeHGlobal(buffer);
			}

			return null;
			//ClosePrinter((int)_printerHandle);

		}
		#region Callback Function

		/// <summary>
		/// Отлавливается нужное задание и вызывается событие, на которое подписались в вызывающем коде
		/// </summary>
		/// <param name="state"></param>
		/// <param name="timedOut"></param>
		internal void PrinterNotifyWaitCallback(object state, bool timedOut)
        {
            if (_printerHandle == IntPtr.Zero) return;

            #region read notification details

            _notifyOptions.Count = 1;
            var pdwChange = 0;
            IntPtr pNotifyInfo;
            var bResult = FindNextPrinterChangeNotification(_changeHandle, out pdwChange, _notifyOptions,
                out pNotifyInfo);
	        _notifyOptions.dwFlags = 0;
            //If the Printer Change Notification Call did not give data, exit code
            if ((bResult == false) || (((int) pNotifyInfo) == 0)) return;

            //If the Change Notification was not relgated to job, exit code
            var bJobRelatedChange = ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_ADD_JOB) ==
                                     PRINTER_CHANGES.PRINTER_CHANGE_ADD_JOB) ||
                                    ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_SET_JOB) ==
                                     PRINTER_CHANGES.PRINTER_CHANGE_SET_JOB) ||
                                    ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_DELETE_JOB) ==
                                     PRINTER_CHANGES.PRINTER_CHANGE_DELETE_JOB) ||
                                    ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_WRITE_JOB) ==
                                     PRINTER_CHANGES.PRINTER_CHANGE_WRITE_JOB);
            if (!bJobRelatedChange) return;

            #endregion

            #region populate Notification Information

            //Now, let us initialize and populate the Notify Info data
            var info = (PRINTER_NOTIFY_INFO) Marshal.PtrToStructure(pNotifyInfo, typeof (PRINTER_NOTIFY_INFO));
            var pData = (long) pNotifyInfo + (long) Marshal.OffsetOf(typeof (PRINTER_NOTIFY_INFO), "aData");
            var data = new PRINTER_NOTIFY_INFO_DATA[info.Count];
            for (uint i = 0; i < info.Count; i++)
            {
                data[i] =
                    (PRINTER_NOTIFY_INFO_DATA) Marshal.PtrToStructure((IntPtr) pData, typeof (PRINTER_NOTIFY_INFO_DATA));
                pData += Marshal.SizeOf(typeof (PRINTER_NOTIFY_INFO_DATA));
            }

            #endregion

            #region iterate through all elements in the data array

            for (var i = 0; i < data.Count(); i++)
            {
				if (data[i].Field == (ushort)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_STATUS
					&&
					(data[i].Type == (ushort)PRINTERNOTIFICATIONTYPES.JOB_NOTIFY_TYPE ||
					data[i].Type == (ushort)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_TOTAL_PAGES)
				    )
				{
                    var jStatus = (JOBSTATUS) Enum.Parse(typeof (JOBSTATUS), data[i].NotifyData.Data.cbBuf.ToString());
                    var intJobId = (int) data[i].Id;
                    string strJobName;
                    PrintSystemJobInfo pji = null;

                    try
                    {
                        _spooler = new PrintQueue(new PrintServer(), SpoolerName);
                        pji = _spooler.GetJob(intJobId);
                        if (!_objJobDict.ContainsKey(intJobId))
                            _objJobDict[intJobId] = pji.Name;
                        strJobName = pji.Name;
                    }
                    catch(Exception ex)
                    {
                        pji = null;
                        _objJobDict.TryGetValue(intJobId, out strJobName);
                        if (strJobName == null) strJobName = string.Empty;
                    }

                    if (OnJobStatusChange != null)
                    {
                        //Let us raise the event calls: pqm_OnJobStatusChange
                        JobDetail jobDetails = GetJobDetail(data[i].Id);//Get detalis about job
                        OnJobStatusChange(this, new PrintJobChangeEventArgs(intJobId, strJobName, jStatus, jobDetails, pji)); //Set pause for job
	                    
                    }
                }
            }

            #endregion

            #region reset the Event and wait for the next event

            _mrEvent.Reset();
            _waitHandle = ThreadPool.RegisterWaitForSingleObject(_mrEvent, PrinterNotifyWaitCallback, _mrEvent, -1, true);

            #endregion
        }
		
		#endregion

		

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct JOB_INFO_2
		{
			public UInt32 JobId;
			public IntPtr pPrinterName;
			public IntPtr pMachineName;
			public IntPtr pUserName;
			public IntPtr pDocument;
			public IntPtr pNotifyName;
			public IntPtr pDatatype;
			public IntPtr pPrintProcessor;
			public IntPtr pParameters;
			public IntPtr pDriverName;
			public IntPtr pDevMode;
			public IntPtr pStatus;
			public IntPtr pSecurityDescriptor;
			public UInt32 Status;
			public UInt32 Priority;
			public UInt32 Position;
			public UInt32 StartTime;
			public UInt32 UntilTime;
			public UInt32 TotalPages;
			public UInt32 Size;
			public SYSTEMTIME Submitted;
			public UInt32 Time;
			public UInt32 PagesPrinted;

		}

		#region DLL Import Functions

		[DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool OpenPrinter(string pPrinterName,
            out IntPtr phPrinter,
            int pDefault);


        [DllImport("winspool.drv", EntryPoint = "ClosePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        internal static extern bool ClosePrinter
            (int hPrinter);

        [DllImport("winspool.drv",
            EntryPoint = "FindFirstPrinterChangeNotification",
            SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr FindFirstPrinterChangeNotification
            ([In] IntPtr hPrinter,
                [In] int fwFlags,
                [In] int fwOptions,
                [In, MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions);

        [DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification",
            SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = false,
            CallingConvention = CallingConvention.StdCall)]
        internal static extern bool FindNextPrinterChangeNotification
            ([In] IntPtr hChangeObject,
                [Out] out int pdwChange,
                [In, MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions,
                [Out] out IntPtr lppPrinterNotifyInfo
            );

		[DllImport("winspool.drv", EntryPoint = "GetJob",
	SetLastError = true,
	ExactSpelling = false,
	CallingConvention = CallingConvention.StdCall)]
		internal static extern bool GetJob
		([In] IntPtr hPrinter,
		[In] UInt32 jobId,
		[In] UInt32 level,
		[Out] IntPtr pJob,
		[In] UInt32 cbBuf,
		[Out] out UInt32 pcbNeeded
    );
		#endregion

		#region private variables

		private IntPtr _printerHandle = IntPtr.Zero;
        internal string SpoolerName;
        private readonly ManualResetEvent _mrEvent = new ManualResetEvent(false);
        private RegisteredWaitHandle _waitHandle;
        private IntPtr _changeHandle = IntPtr.Zero;
        private readonly PRINTER_NOTIFY_OPTIONS _notifyOptions = new PRINTER_NOTIFY_OPTIONS();
        private readonly Dictionary<int, string> _objJobDict = new Dictionary<int, string>();
        private PrintQueue _spooler;

        #endregion
    }
}
