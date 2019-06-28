using System;
using System.Linq;
using System.Management;
using System.Printing;
using System.Runtime.InteropServices;
using EventHook;
using Serilog;

namespace Project53
{
    public class JobController
    {
        private const string JobTicket = "_jobticket_";
        private string[] _printers;
        private ILogger _logger;
        
        public JobController(string[] printers, ILogger logger)
        {
            _printers = printers;
            _logger = logger;
        }
        
        public void StartWatching()
        {
            try
            {
                _logger.Information(LogMessages.PrinterJobWatchingWasStarted);
                _logger.Debug(LogMessages.WorkWithPrinters);
                foreach (var p in _printers)
                {
                    _logger.Debug(p);
                    
                    //Подключение на оповещения от принтера, отлавливается корректное задание, затем вызывается метод, который ниже подписывается на событие
                    PrintWatcher.Start(p); //Это из либы с GitHub
                    PrintWatcher.OnPrintEvent += PrintWatcherOnOnPrintEvent;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while starting the job tracking feature");
            }
        }

        /// <summary>
        /// It's main handler of printers queue hooks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintWatcherOnOnPrintEvent(object sender, PrintEventArgs e)
        {
            _logger.Information(LogMessages.JobWasCatched);

            bool isManagedDoc = true; //If the document is fully managed, for example, it is txt or image
            if (IsThisMsOfficeDoc(e.EventData))//Unmanaged doc, for example msoffice
                isManagedDoc = ValidateMsOfficeTicket(e);//If msoffice doc have ticket it's means that the document is management
            
//            if (!isManagedDoc)//Unmanaged doc - MsOffice without ticket
//            {
//                //CancelDocumentFromPrintQueue(e);
//            }else if (isManagedDoc && e.EventData.Ji.IsSpooling)
            {
                _logger.Information("Управляемый документ.");
                _logger.Debug($"Страниц {e.JobId}; копий {e.EventData.JobDetail.DevMode.dmCopies}");
                
                _logger.Information("Запрос аккаунта."); 
                _logger.Information("Проверка баланса.");
                _logger.Information($"Требуется {e.EventData.Ji.NumberOfPages * e.EventData.JobDetail.DevMode.dmCopies * 2} у.е..");
                _logger.Information("Печать разрешена.");
                _logger.Information("Запуск печати.");
                _logger.Information("Отправка данных на сервер.");
                _logger.Information("Очищаем данные авторизации.");
            }
        }

        private void CancelDocumentFromPrintQueue(PrintEventArgs e)
        {
            _logger.Information("Неуправляемый документ.");
            PrintLog(e.EventData);
            e.EventData.Ji.Cancel();
            e.EventData.Ji.Dispose();
            _logger.Information(LogMessages.PrinterJobWasCancel);
        }

        private string[] _msOfficeExtensions = new string[2]{".doc", ".docx"};
        private bool IsThisMsOfficeDoc(PrintEventData eEventData)
        {
            return _msOfficeExtensions.Any(el => eEventData.FileName.Contains(el));
        }

        private bool ValidateMsOfficeTicket(PrintEventArgs p)
        {
            var ticket = (string) p.EventData.Ji.PropertiesCollection[JobTicket];

            _logger.Debug("Извлечен тикет из документа:");
            _logger.Debug(ticket);
            
            bool ticketOfMsOfficeIsCorrect = TicketRegistry.ValidateAndRemove(ticket);

            if (ticketOfMsOfficeIsCorrect)
            {
                _logger.Debug("Тикет прошел валидацию, удален из хранилища тикетов");
                return true;
            }

            return false;
        }
//
//        private PrintEventArgs UpdateJobPropertyAboutValid()
//        {
//            
//        }
        
        private void PrintLog(PrintEventData job)
        {
            _logger.Debug("Найдено задание принтера:");
            _logger.Debug(job.PrinterName);
            _logger.Debug("Будет отменено следующее задание:");
            _logger.Debug(job.JobName);
            _logger.Debug("Имя документа:");
            _logger.Debug(job.FileName);

        }
        
        
        
    }
}