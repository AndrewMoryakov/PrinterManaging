using System;
using System.Printing;
using EventHook;

namespace Project53.New_arhtech
{
    public class PrinterWrapper
    {
        private Action<JobMeta> _action;
        public PrinterWrapper(string title)
        {
            Title = title;
            PrintWatcher.Start(Title); //Это из либы с GitHub
            PrintWatcher.OnPrintEvent += PrintWatcherOnPrintEvent;
        }

        public string Title { get; private set; }

        public void SubscribeOnPrintEvent(Action<JobMeta> subscriber)
        {
            _action = subscriber;
        }

        private void PrintWatcherOnPrintEvent(object sender, PrintEventArgs e)
        {
            if(e.EventData.JobDetail.JobInfo2.TotalPages != 0)
            _action(new JobMeta(
                e.EventData.FileName,
                (int)e.EventData.JobDetail.JobInfo2.TotalPages,
                e.EventData.JobDetail.DevMode.dmCopies
            ));
        }
    }
}