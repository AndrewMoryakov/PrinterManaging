using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Threading.Tasks;
using BackendClient;
using ConsoleApp1_2;
using DreamPlace.Lib.Rx;
using Project53.New_arhtech;
using Project53.New_arhtech.Http.RedServer;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog.Sinks.Telegram;
using Task = System.Threading.Tasks.Task;

namespace Project53
{
    //https://stackoverflow.com/questions/6103705/how-can-i-send-a-file-document-to-the-printer-and-have-it-print
    internal class Program
    {
        private static Logger _logger;
        public static List<string> PrintingMsDocs;
        private static BaseValidationOfDoc _validator;
        private static IDisposable _disposableserver;
        public static Server s;
        private static ClientToBack _clientToBack;
        
        private static void Configure()
        {
            _validator = new BaseValidationOfDoc();
            PrintingMsDocs = new List<string>();
            _logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored)
                .WriteTo.File("logs.log")
                // .MinimumLevel.Verbose()
                .WriteTo.Telegram("1206276933:AAE5Iinp03S5l5bveJ_s4QjF8YYjuUcankc", "61280592", restrictedToMinimumLevel: LogEventLevel.Error)
                .CreateLogger();
            
            var configuration = GetAppsettingsReader();

            _clientToBack = new ClientToBack(
                configuration.GetSection("appSettings:serviceDomain").Value
                // configuration.GetSection("appSettings:printControllerHost").Value
            );
            
            Registry.Public(_logger);
            Registry.Public(_clientToBack);
            
            s = new Server();
            s.Start();
            // _disposableserver = RestFullServer.StartServer();
        }

        private static IConfigurationRoot GetAppsettingsReader()
        {
            var builder = new ConfigurationBuilder()
                // .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            return configuration;
        }

        public static void Main(string[] args)
        {
            Configure();
            ProcessWithPrinters();
            #if DEBUG
                _logger.Information("DEBUG");
                Registry.Subscribe<string>(el =>
                {
                    _logger.Information("login");
                    continueClicked.TrySetResult(null);
                }, RegistryAddresses.Login);
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
                    var printer = new PrinterWrapper(printerName, _logger);
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
        
        private static TaskCompletionSource<object> continueClicked;
        private static HashSet<string> _jobs = new HashSet<string>();
        private static void Subscriber(JobMeta job)
        {
            var isPaused = job._job.IsSpooling == false && job._job.IsPaused == true;
            var alreadyProcessedJob = _jobs.Contains(job.Guid) == true;
            if (isPaused && !alreadyProcessedJob)
            {
                _jobs.Add(job.Guid); //ToDo одно задание сюда попадает множество раз. Для предотвращения кроме имени документа нужно генерировать уникальный хэш
            }
            else
            {
                // Console.ForegroundColor = ConsoleColor.Cyan;
                // Console.WriteLine($"Process aborted: {job._job.JobStatus}");
                // Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            
            WriteLogs(job);
           
            var jobIsValid = _validator.ApplyChecks(job);
            if (jobIsValid == false)
                job.CancelJob();
            LaunchAuthGui().Wait();
            
            Client client = Auth.GetClient();
            var canBePrinted = ClientValidator.CanContinuePrinting(client, job);
            if (canBePrinted)
                RunPrintJob(job._job);
            else
                job._job.Cancel();
            
            WriteLogsAboutJobAndClient(job, client, canBePrinted);

            // RemoveJobIfPaused(job, isPaused);
        }

        private static async System.Threading.Tasks.Task LaunchAuthGui()
        {
            _logger.Information("Start gui");
            await GetResults();
            _logger.Information("Get AUTH after waiting!!!!!!");
        }
        
        private static async Task GetResults()
        {
            var startInfo = 
                new ProcessStartInfo(@"C:\Users\hopt\RiderProjects\PrinterManaging\DesctopGui\bin\Debug\netcoreapp3.1\DesctopGui.exe");
            Process.Start(startInfo);
            continueClicked = new TaskCompletionSource<object>();
            await continueClicked.Task;
        }

        private static void WriteLogsAboutJobAndClient(JobMeta job, Client client, bool canBePrinted)
        {
            _logger.Information($"Client: {client.Email}, balance: {client.Balance}");
            _logger.Information($"Job guid: {job.Guid}");
            _logger.Information($"Balance required: {(job.Copies * job.CountOfPages) * 2m}");
            _logger.Information($"Has ability to be printed: {canBePrinted}");
        }

        private static void RemoveJobIfPaused(JobMeta job, bool isPaused)
        {
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
            jobJob.Resume();
            _logger.Information("Start printing");
            _logger.Information("___");
        }

        private static void WriteLogs(JobMeta job)
        {
            _logger.Information($"***********************************************");
            _logger.Information($"File to print: {job.DocumentName}");
            _logger.Information($"Job state: {job._job.JobStatus.ToString()}");
            _logger.Information($"Amount of pages: {job.CountOfPages}");
            _logger.Information(
                $"Amount of copies: {job._job.HostingPrintQueue.CurrentJobSettings.CurrentPrintTicket.CopyCount}");
        }
    }
}