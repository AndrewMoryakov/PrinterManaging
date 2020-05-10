using System;
using Microsoft.Office.Interop.Word;

namespace Project53.Models
{
	public class FileInfoEventArgs : EventArgs
	{
		//public FileInfoEventArgs(Document doc, string fullFileName, string fileExtension, int count)
		//{
		//	this.FullFileName = fullFileName;
		//	this.FileExtension = fileExtension;
		//	FileName = Path.GetFileName(FullFileName);
			
		//	this.PagesCount = count;
		//	Doc = doc;
		//}

		public FileInfoEventArgs(string fullFileName, int processId, string fileExtension, int count)
		{
			this.FullFileName = fullFileName;
			this.FileExtension = fileExtension;
			this.PagesCount = count;
			this.ProcessId = processId;
		}

		public Document Doc { get; set; }
		public string FullFileName { get; }
		public string FileExtension { get; }
		public int PagesCount { get; set; }
		public int ProcessId { get; set; }
		public string FileName { get; private set; }

		public int GetActualCountOfPages()
		{
			return Doc.ComputeStatistics(WdStatistic.wdStatisticPages, false);
		}
	}
}