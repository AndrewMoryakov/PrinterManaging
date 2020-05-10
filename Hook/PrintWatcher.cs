﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Management;
using System.Printing;
using EventHook.Hooks;
using EventHook.Helpers;
using EventHook.Hooks.Library;
using System.Runtime.InteropServices;

namespace EventHook
{
    /// <summary>
    /// A object holding key information on a particular print event
    /// </summary>
    public class PrintEventData
    {
        public DateTime EventDateTime { get; set; }
        public string PrinterName { get; set; }
        public string AppTitle { get; set; }
        public string JobName { get; set; }
        public string UnicJobId { get; set; }
        public int? Pages { get; set; }
        public int? JobSize { get; set; }
		public int ProcessId { get; set; }
		public string FileName { get; set; }
		public PrintSystemJobInfo Ji { get; set; }
		public JobDetail JobDetail { get; set; }
    }

    /// <summary>
    /// An argument passed along user call backs
    /// </summary>
    public class PrintEventArgs : EventArgs
    {
		public int JobId { get; set; }
        public PrintEventData EventData { get; set; }
    }

    /// <summary>
    /// A class that wraps around printServer object
    /// </summary>
    public class PrintWatcher
    {
        /*Print history*/
        private static bool isRunning;
        private static object accesslock = new object();

        private static ArrayList printers = null;
        private static PrintServer printServer = null;

        public static event EventHandler<PrintEventArgs> OnPrintEvent;

        /// <summary>
        /// Start watching print events
        /// </summary>
        public static void Start(string printerName)
        {
            //if (!isRunning)
            {
                lock (accesslock)
                {
                    printers = new ArrayList();
                    printServer = new PrintServer();

                    PrintQueue printQueue = printServer.GetPrintQueue(printerName);
                    var pqHook = new PrintQueueHook(printQueue.Name);
                    pqHook.OnJobStatusChange += pqm_OnJobStatusChange;
                    pqHook.Start();
                    printers.Add(pqHook);

                    isRunning = true;
                }
            }
        }

        /// <summary>
        /// Stop watching print events
        /// </summary>
        public static void Stop()
        {
            if (isRunning)
            {
                lock (accesslock)
                {
                    if (printers != null)
                    {
                        foreach (PrintQueueHook pqm in printers)
                        {
                            pqm.OnJobStatusChange -= pqm_OnJobStatusChange;

                            try
                            {
                                pqm.Stop();
                            }
                            catch
                            {
                                //ignored intentionally
                                //Not sure why but it throws error
                                //not a bug deal since we a stopping it anyway
                            }
                        }
                        printers.Clear();
                    }
                    printers = null;
                    isRunning = false;
                }
            }
        }

        /// <summary>
        /// Invoke user callback as soon as a relevent event is fired
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void pqm_OnJobStatusChange(object sender, PrintJobChangeEventArgs e)
        {
            // if ((e.JobStatus & JOBSTATUS.JOB_STATUS_PAUSED) == JOBSTATUS.JOB_STATUS_PAUSED && e.JobInfo != null)
            {
                var hWnd = WindowHelper.GetActiveWindowHandle();
                var appTitle = WindowHelper.GetWindowText(hWnd);
                var appName = WindowHelper.GetAppDescription(WindowHelper.GetAppPath(hWnd));

				GetWindowThreadProcessId(hWnd, out int procId);

				dynamic printEvent = null;
//				try
//				{
					printEvent = new PrintEventData()
					{
						AppTitle = appTitle,
						UnicJobId = e.UnicIdentOfJob,
						JobName = e.JobInfo?.JobName,
						JobSize = e.JobInfo?.JobSize,
						EventDateTime = DateTime.Now,
						Pages = e.JobInfo?.NumberOfPages,
						PrinterName = ((PrintQueueHook) sender).SpoolerName,
						FileName = e.JobInfo?.Name,
						Ji = e.JobInfo,
						ProcessId = procId,
						JobDetail = e.JobDetail
					};
//				}
//				catch (Exception ex)
//				{
//					
//				}
	            var d = PausePrintJob(((PrintQueueHook) sender).SpoolerName, e.JobId);
	            Console.ForegroundColor = ConsoleColor.Yellow;
	            Console.WriteLine(d);
	            Console.ForegroundColor = ConsoleColor.White;
	            
	            if(d == true)
					OnPrintEvent?.Invoke(null, new PrintEventArgs() { EventData = printEvent }); //Поставлено на паузу
            }
            
            	
            // e.JobInfo?.Pause();
        }
        
        public static bool PausePrintJob(string printerName, int printJobID)
        {
	        bool isActionPerformed = false;
	        string searchQuery = "SELECT * FROM Win32_PrintJob";
	        ManagementObjectSearcher searchPrintJobs = 
		        new ManagementObjectSearcher(searchQuery);
	        ManagementObjectCollection prntJobCollection = searchPrintJobs.Get();
	        foreach(ManagementObject prntJob in prntJobCollection)
	        {
		        string jobName = prntJob.Properties["Name"].Value.ToString();
		        
		        if(prntJob.Properties["JobStatus"].Value != null)
					Console.WriteLine(prntJob.Properties["JobStatus"].Value);
		        
		        //Job name would be of the format [Printer name], [Job ID]
		        char[] splitArr = new char[1];
		        splitArr[0] = Convert.ToChar(",");
		        string prnterName = jobName.Split(splitArr)[0];
		        int prntJobID = Convert.ToInt32(jobName.Split(splitArr)[1]);
		        string documentName = prntJob.Properties["Document"].Value.ToString();
		        if(String.Compare(prnterName, printerName, true) == 0)
		        {
			        if(prntJobID == printJobID)
			        {
				        prntJob.InvokeMethod("Pause", null);
				        isActionPerformed = true; 
				        break;
			        }
		        }
	        }
	        return isActionPerformed;
        }

		[DllImport("user32.dll")]
		static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

	}
}
