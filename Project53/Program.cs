using System;
using Serilog;

namespace Project53
{
    internal class Program
    {
        public static void Main(string[] args)
        {   
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
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