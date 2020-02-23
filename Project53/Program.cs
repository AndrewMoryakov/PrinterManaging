using System;
using System.Collections.Generic;
using Project53.New_arhtech;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Project53
{
//https://stackoverflow.com/questions/6103705/how-can-i-send-a-file-document-to-the-printer-and-have-it-print
    internal class Program
    {
        public static List<string> PrintingMsDocs = new List<string>(); 
        public static void Main(string[] args)
        {   
            var logger = new LoggerConfiguration()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored)
                .WriteTo.File("logs.log")
                .MinimumLevel.Debug()
                .CreateLogger();
            
            var supprtedPrinters = Printers.Get();

            foreach (var printerName in supprtedPrinters)
            {
                var printer = new PrinterWrapper(printerName);
                printer.SubscribeOnPrintEvent(Subscriber);
            }
            
            //var printers;
            
            //RunSpoolerService
            //SubscribeToOwnServer
            // var msWordHandler = new MsWordHandle(logger);
            // var supprtedPrinters = Printers.Get();
            // JobController jobController = new JobController(supprtedPrinters, logger);
            // jobController.StartWatching();

            Console.ReadKey();
        }

        private static void Subscriber(JobMeta obj)
        {
        }
    }
}