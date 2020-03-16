using System;
using System.Printing;

namespace Project53.New_arhtech
{
    public class JobMeta
    {
        public PrintSystemJobInfo _job; 
        public string DocumentName { get; private set; }
        public int CountOfPages { get; private set; }
        public int Copies { get; private set; }

        public void CheckJob()
        {
            if(_job == null)
                throw new ArgumentNullException("Задание на печать не инициализировано.");
        }

        public void CancelJob()
        {
            CheckJob();
            _job.Cancel();
        }

        public void Resume()
        {
            CheckJob();
            _job.Resume();
        }

        public JobMeta(string documentName, int countOfPages, int copies, PrintSystemJobInfo job)
        {
            _job = job;
            DocumentName = documentName;
            CountOfPages = countOfPages;
            Copies = copies;
        }

        public void ToPause()
        {
            CheckJob();
            _job.Pause();
        }
    }
}