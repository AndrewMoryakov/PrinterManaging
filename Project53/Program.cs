﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Printing;
using Newtonsoft.Json;
using Project53.New_arhtech;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace Project53
{
//https://stackoverflow.com/questions/6103705/how-can-i-send-a-file-document-to-the-printer-and-have-it-print
    internal class Program
    {
        private static Logger _logger;
        public static List<string> PrintingMsDocs;
        private static BaseValidationOfDoc _validator;  

        private static void Configure()
        {
            _validator = new BaseValidationOfDoc();
            PrintingMsDocs = new List<string>();
            _logger = new LoggerConfiguration()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored)
                .WriteTo.File("logs.log")
                .MinimumLevel.Verbose()
                .CreateLogger();
        }

        public static void Main(string[] args)
        {
            Configure();
            ProcessWithPrinters();
            
            #if DEBUG
                _logger.Information("DEBUG");
            #else
                _logger.Information("RELEASE");
            #endif
            Console.ReadKey();
        }

        private static void ProcessWithPrinters()
        {
            List<string> GetPrinters()
            {
                var list = Printers.Get().ToList();
                _logger.Information("Printers");
                list.ForEach(el => _logger.Information(el));
                return list;
            }
            
            void SubscribePrinters(List<string> supprtedPrinters1)
            {
                foreach (var printerName in supprtedPrinters1)
                {
                    var printer = new PrinterWrapper(printerName);
                    printer.SubscribeOnPrintEvent(Subscriber);
                }
            }
            
            var supprtedPrinters = GetPrinters();
            SubscribePrinters(supprtedPrinters);
        }

        private void StartOldFunctions()
        {
            //var printers;
            
            //RunSpoolerService
            //SubscribeToOwnServer
            // var msWordHandler = new MsWordHandle(logger);
            // var supprtedPrinters = Printers.Get();
            // JobController jobController = new JobController(supprtedPrinters, logger);
            // jobController.StartWatching();
        }
        
        private static HashSet<string> _jobs = new HashSet<string>();
        private static void Subscriber(JobMeta job)
        {
            var isPaused = job._job.IsSpooling == false && job._job.IsPaused == true;
            if (_jobs.Contains(job.Guid) == false && isPaused)
                _jobs.Add(job.Guid); //ToDo одно задание сюда попадает множество раз. Для предотвращения кроме имени документа нужно генерировать уникальный хэш
            else
                return;
            
            LogIt(job);
           
            var jobIsValid = _validator.ApplyChecks(job);

            if (jobIsValid == false)
                job.CancelJob();

            var client = Auth.GetClient();
            _logger.Information($"Client: {client.Email}, balance: {client.Balance}");
            _logger.Information($"Job guid: {job.Guid}");

            var canBePrinted = ClientValidator.CanContinuePrinting(client, job);

            if (canBePrinted)
                RunPrintJob(job._job);
            else
                job._job.Cancel();

            _logger.Information($"Balance required: {(job.Copies * job.CountOfPages) * 2m}");
            _logger.Information($"Has ability to be printed: {canBePrinted}");

            
            if (isPaused)
            {
                _jobs.Remove(job.Guid);

                try
                {
                    job._job.Cancel();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Не удалось удалить задание печати");
                }
            }
        }
//Todo если этот сервис отключается, то удаляем все задания печати.
        private static void RunPrintJob(PrintSystemJobInfo jobJob)
        {
            _logger.Information("Start print");
        }

        private static void LogIt(JobMeta job)
        {
            _logger.Information($"File to print: {job.DocumentName}");
            _logger.Information($"Job state: {job._job.JobStatus.ToString()}");
            _logger.Information($"Amount of pages: {job.CountOfPages}");
            _logger.Information(
                $"Amount of copies: {job._job.HostingPrintQueue.CurrentJobSettings.CurrentPrintTicket.CopyCount}");
        }
    }
}