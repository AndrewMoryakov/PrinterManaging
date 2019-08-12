using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Project53
{
	/// <summary>
	/// it's just wrapper
	/// </summary>
	public class AboutPagesOfDocument
	{
		private string _pages;
		private string _selectPages;
		private string _from;
		private string _to;
		private string _copies;
		
		public AboutPagesOfDocument(string pages, string selectPages, string from, string to, string copies)
		{
			_pages = pages;
			_selectPages = selectPages;
			_from = from;
			_to = to;
			_copies = copies;
		}

		public override string ToString()
		{
			return $"{_pages} {_selectPages} {_from} {_to} {_copies}";
		}
	}
	
	/// <summary>
	/// Container for files that want to be printed
	/// </summary>
	public static class FilesWaitingPrinting
	{
		private static List<string> _filesThatWaitPrinting;
		/// <summary>
		/// Добавляет файл, который можно напечатать.
		/// </summary>
		/// <param name="fileName">Имя файла, если присутствует путь к файлу, он будет обрезан.</param>
		public static void Add(string fileName, AboutPagesOfDocument pages)
		{
			_filesThatWaitPrinting = new List<string>();
			string fileNameWithourtPath = Path.GetFileName(fileName);
			string fileId = $"{fileName} {pages.ToString()}";
			
			_filesThatWaitPrinting.Add(fileId);
		}

		/// <summary>
		/// Истина, если содержится описание указанного файла.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="pages"></param>
		/// <returns></returns>
		public static bool Contains(string fileName, AboutPagesOfDocument pages)
		{
			if (!_filesThatWaitPrinting.Any())
				return false;
			
			string fileNameWithourtPath = Path.GetFileName(fileName);
			string fileId = $"{fileName} {pages.ToString()}";

			return _filesThatWaitPrinting.Contains(fileId);
		}

		/// <summary>
		/// Удаляет информацию о файле, который нужно напечатать. После удаления запись безвозратно удаляется и возвращается истина.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="pages"></param>
		/// <returns>Истина если запись успешно удалена, то есть она существовала - была добавлена на ожидание печати..</returns>
		public static bool Remove(string fileName, AboutPagesOfDocument pages)
		{
			string fileId = $"{fileName} {pages.ToString()}";
			
			if (Contains(fileName, pages))
			{
				_filesThatWaitPrinting.Remove(fileId);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}