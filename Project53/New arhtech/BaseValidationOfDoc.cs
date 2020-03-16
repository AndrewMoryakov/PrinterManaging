using System.IO;
using System.Linq;
using Project53.New_arhtech;

namespace Project53
{
	public class BaseValidationOfDoc
	{
		private string[] _extensionsForBlock;

		public BaseValidationOfDoc()
		{
			_extensionsForBlock = new[] {".doc", ".excel"};
		}
		
		public bool CheckJob(JobMeta job)
		{
			bool isValidExtension = CheckExtensions(job.DocumentName);

			return isValidExtension;
		}

		private bool CheckExtensions(string fileName)
		{
			return _extensionsForBlock.Any(el => el != Path.GetExtension(fileName));
		}
	}
}