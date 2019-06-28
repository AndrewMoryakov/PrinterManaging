using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Project53.Models;
using Serilog;

namespace Project53
{
	public class MsWordHandle
	{
		[DllImport("user32.dll")]
		static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

		private ILogger _logger;
		public MsWordHandle(ILogger logger)
		{
			_logger = logger;
			WatchForProcessStart("WINWORD.EXE");
		}
		
		private ManagementEventWatcher WatchForProcessStart(string processName)
		{
			string queryString =
				"SELECT TargetInstance.Name" +
				"  FROM __InstanceCreationEvent " +
				"WITHIN  1 " +
				" WHERE TargetInstance ISA 'Win32_Process' " +
				"   AND TargetInstance.Name = '" + processName + "'";

			// The dot in the scope means use the current machine
			string scope = @"\\.\root\CIMV2";

			// Create a watcher and listen for events
			ManagementEventWatcher watcher = new ManagementEventWatcher(scope, queryString);
			watcher.EventArrived += ProcessStarted;
			watcher.Start();
			return watcher;
		}
		
		protected void ProcessStarted(object sender, EventArrivedEventArgs e)
		{

			ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
			int id = int.Parse(targetInstance["ProcessId"].ToString());

			var path = GetPathOfOpenedDoc(targetInstance, id);
			Process.GetProcessById(id).Kill();


			Document reloadedDoc = GetReloadedFile(path);
			//reloadedDoc.Close(false, null, null);
			//reloadedDoc = GetReloadedFile(path);
			//return;

			GetWindowThreadProcessId(reloadedDoc.Application.ActiveWindow.Hwnd, out int reloadedProcId);
			FileInfoEventArgs fInfo = new FileInfoEventArgs(path, reloadedProcId, Path.GetExtension(path), 0);

			reloadedDoc.Content.Application.DocumentBeforePrint += delegate(Document doc, ref bool cancel)
			{
				
			};
			SetDocumentInfo(reloadedDoc, fInfo);

			OpenWordPrintDialog(reloadedDoc);

			//msWordDoc.Close(0, null, null);

			//msWordDoc.Application.Quit(0, null, null);

			//if (бабок хватает)
			//msOfficeApp.Print();
			//else
			//{
			//	запускаем авторизацию
			//}

		}
		
		private void SetDocumentInfo(Document msWordDoc, FileInfoEventArgs fInfo)
		{
			//ToDo (d.Application.StatusBar = $"Reloaded";) тут я каким-то образом должен к загаловку окна добавить уникальный идентификатор и когда юзер нажмет кнопку печати, я получу этот идентификатор и буду точно знать какой документ печатается
			int countOfPage = msWordDoc.ComputeStatistics(WdStatistic.wdStatisticPages, false);
			fInfo.Doc = msWordDoc;
			fInfo.PagesCount = countOfPage;
		}
		
		private void OpenWordPrintDialog(Document d)
		{
			PrintDialog pDialog = new PrintDialog();
			//pDialog.Document.PrinterSettings.PrinterName = printer;
			pDialog.PrinterSettings.Copies = 10;
			pDialog.Document = new PrintDocument();
			
			pDialog.PrinterSettings.PrintRange = PrintRange.SomePages;
			pDialog.AllowSomePages = true;
			pDialog.AllowSelection = true;
			
			

			if (pDialog.ShowDialog() == DialogResult.OK)
			{
				d.Application.ActivePrinter = pDialog.PrinterSettings.PrinterName;
				d.Activate();

				object back = false;
				object append = true;
				object copies = pDialog.PrinterSettings.Copies.ToString();
				object from = pDialog.PrinterSettings.FromPage.ToString();
				object to = pDialog.PrinterSettings.ToPage.ToString();
				
				_logger.Debug("Диалог печати - ОК. Количество копий {copies}, листы от {from}, до {to}");
				
				d.Application.ActiveDocument.PrintOut(
					Background: ref back,
					Range: Microsoft.Office.Interop.Word.WdPrintOutRange.wdPrintFromTo
					,Item: Microsoft.Office.Interop.Word.WdPrintOutItem.wdPrintDocumentContent
					,PageType:Microsoft.Office.Interop.Word.WdPrintOutPages.wdPrintAllPages
					,Append: ref append,
					Copies: ref copies
					,From: ref from 
					,To: ref to);
				
				//this will also work: doc.PrintOut();
				d.Close(SaveChanges: false);
				//d = null;
			}
		}
		
		private Document GetReloadedFile(string fullFileName)
        		{
        			//string docName = Path.GetFileName(fileInfo.FullFileName);
        			//KillProccesed(docName, "WINWORD");
        			//start 2
        			if (fullFileName.Contains(Environment.CurrentDirectory))
        				throw new Exception();
        
        
        			var d = System.Runtime.InteropServices.Marshal
        					.BindToMoniker(fullFileName) as
        				Microsoft.Office.Interop.Word.Document;
        
        			d.Application.StatusBar = $"Reloaded {fullFileName}";
        			d.Application.Documents.Open(fullFileName);
        
        			//var numberOfPages = d.ComputeStatistics(WdStatistic.wdStatisticPages, false);
        
        
        
        			//d.Application.Quit(0, null, null);
        			//end 2
        
        			return d;
        
        			////var application = new Application();
        			////string tempFileName = GetTempFileName(docPath);
        			////System.IO.File.Copy(docPath, tempFileName);
        
        
        			////var pathCurrDoc = Path.Combine(Environment.CurrentDirectory, tempFileName);
        			////var document = application.Documents.Open(pathCurrDoc, AddToRecentFiles: false);
        
        			////var numberOfPages = document.ComputeStatistics(WdStatistic.wdStatisticPages, false);
        			////document.Close(0, null, null);
        
        			////application.Quit(0, null, null);
        			////System.IO.File.Delete(pathCurrDoc);
        
        			////return numberOfPages;
        		}
		
		private static string GetPathOfOpenedDoc(ManagementBaseObject targetInstance, int processId)
		{
			
			var commandLine = targetInstance.Properties["CommandLine"].Value;
			while (commandLine == null)
			{
				commandLine = targetInstance.Properties["CommandLine"].Value;
			}

			string path = Regex.Match((string)commandLine, ".\\/n \"(?<path>.+\") \\/o \"").Groups["path"].Value;
			path = path.Remove(path.Length - 1);
			return path;
		}
	}
}