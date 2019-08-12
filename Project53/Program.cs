using System;
using System.Collections.Generic;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Project53
{
    internal class Program
    {
        public static List<string> PrintingMsDocs = new List<string>(); 
        public static void Main(string[] args)
        {   
            var logger = new LoggerConfiguration()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored)
                .WriteTo.File("log.txt")
                .MinimumLevel.Debug()
                .CreateLogger();
            
            //RunSpoolerService
            //SubscribeToOwnServer
            var msWordHandler = new MsWordHandle(logger);
            var supprtedPrinters = Printers.Get();
            JobController jobController = new JobController(supprtedPrinters, logger);
            jobController.StartWatching();

            Console.ReadKey();
        }
    }
}