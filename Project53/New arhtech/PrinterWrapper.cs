using System;
using System.Printing;
using EventHook;

namespace Project53.New_arhtech
{
    /// <summary>
    /// Умная обертка для принтера, которая позволяет отслеживать события печати у данного принтера.
    /// И оповещать подписчика, подписанного из вызывающего кода
    /// </summary>
    public class PrinterWrapper
    {
        private Action<JobMeta> _action;
        public string TitleOfPrinter { get; private set; }
        public PrinterWrapper(string titleOfPrinter)
        {
            TitleOfPrinter = titleOfPrinter;
            PrintWatcher.Start(TitleOfPrinter); //Это из либы с GitHub
            PrintWatcher.OnPrintEvent += PrintWatcherOnPrintEvent;
        }

        /// <summary>
        /// Подписывает на событие о старте печати
        /// </summary>
        /// <param name="subscriber">Метод подписчик</param>
        /// <exception cref="Exception">Если метод для подпискик null</exception>
        public void SubscribeOnPrintEvent(Action<JobMeta> subscriber)
        {
            if(subscriber == null)
                throw new Exception("Подписчик не может быть null");
                
            _action = subscriber;
        }

        private void PrintWatcherOnPrintEvent(object sender, PrintEventArgs e)
        {
            if(e.EventData.JobDetail.JobInfo2.TotalPages != 0)
            _action(new JobMeta(
                e.EventData.FileName,
                (int)e.EventData.JobDetail.JobInfo2.TotalPages,
                e.EventData.JobDetail.DevMode.dmCopies,
                e.EventData.Ji
            ));
        }
    }
}