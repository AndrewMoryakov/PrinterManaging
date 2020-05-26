using System;
using System.Printing;
using System.Security.Authentication;
using System.Threading.Tasks;
using EventHook;
using Serilog.Core;

namespace Project53.New_arhtech
{
    /// <summary>
    /// Умная обертка для принтера, которая позволяет отслеживать события печати у данного принтера.
    /// И оповещать подписчика, подписанного из вызывающего кода
    /// </summary>
    public class PrinterWrapper
    {
        private Logger _lgr;
        private Action<JobMeta> _action;
        public string TitleOfPrinter { get; private set; }
        public PrinterWrapper(string titleOfPrinter, Logger lgr)
        {
            TitleOfPrinter = titleOfPrinter;
            PrintWatcher.Start(TitleOfPrinter); //Это из либы с GitHub
            PrintWatcher.OnPrintEvent += PrintWatcherOnPrintEvent;
            _lgr = lgr;
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
            ///ToDo надо добиться того, чтобы приходили только текущие задачи
            var d = PrintWatcher.PausePrintJob(e.EventData.SpoolerName, e.JobId.ToString());
            Console.WriteLine(e.EventData.FileName);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(d?"HOOK: job paused":"HOOK: job NOT PAUSED");
            Console.ForegroundColor = ConsoleColor.White;
            
            try
            {
                e.EventData.Ji.Refresh();
                if (e.EventData.JobDetail != null && e.EventData.JobDetail.JobInfo2.TotalPages != 0)
                    _action(new JobMeta(
                        e.EventData.FileName,
                        e.EventData.UnicJobId,
                        (int) e.EventData.JobDetail.JobInfo2.TotalPages,
                        e.EventData.JobDetail.DevMode.dmCopies,
                        e.EventData.Ji
                    ));
            }
            catch (Exception ex)
            {
                _lgr.Error(ex, "При обработке задания произошла ошибка.");
            }
        }
    }
}