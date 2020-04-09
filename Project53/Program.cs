﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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
            if (_jobs.Contains(job.DocumentName) == false)
                _jobs.Add(job.DocumentName);
            else
                return;
                
            //ToDo он тут много раз вызывается. надо сделать механизм контроля от повторных вызовов одного джоба
           _logger.Information($"File to print: {job.DocumentName}"); 
           _logger.Information($"Job state: {job._job.JobStatus.ToString()}"); 
           _logger.Information($"Amount of pages: {job.CountOfPages}"); 
           _logger.Information($"Amount of copies: {job._job.HostingPrintQueue.CurrentJobSettings.CurrentPrintTicket.CopyCount}");
           // var json = JsonConvert.SerializeObject(job._job); 
            // job.ToPause();
            var jobIsValid = _validator.CheckJob(job);

            if (jobIsValid == false)
                job.CancelJob();

            Client client = Auth.GetClient();

            var canBePrinted = ClientValidator.CanContinuePrinting(client, job);

            // if (canToPrint == true)
                // PausePrintJob(job.) //job.Resume();
        }
    }
}