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
        private static BaseValidationOfDoc _validator;
        private static IDisposable _disposableserver;
        private static ClientToBack _clientToBack;
        public static List<string> PrintingMsDocs;
        public static Server s;

        public static void Main(string[] args)
        {
            Configure();
            SubscribeOnOwnServerEvents();
            ProcessWithPrinters();

#if DEBUG
            _logger.Debug("DEBUG");
#else
            _logger.Debug("RELEASE");
#endif

            Console.ReadKey();
        }

        private static void Configure()
        {
            _validator = new BaseValidationOfDoc();
            PrintingMsDocs = new List<string>();
            _logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored)
                .WriteTo.File("logs.log")
                .MinimumLevel.Information()
                .WriteTo.Telegram("1206276933:AAE5Iinp03S5l5bveJ_s4QjF8YYjuUcankc", "61280592",
                    restrictedToMinimumLevel: LogEventLevel.Error)
                .WriteTo.Telegram("1206276933:AAE5Iinp03S5l5bveJ_s4QjF8YYjuUcankc", "-347387165",
                    restrictedToMinimumLevel: LogEventLevel.Error)
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

        private static void SubscribeOnOwnServerEvents()
        {
            Registry.Subscribe<string>(el =>
            {
                _logger.Debug("login");
                continueClicked.TrySetResult(null);
            }, RegistryAddresses.Login);
            
            Registry.Subscribe<string>(el =>
            {
                if(el.Value == _client.Token)
                    _client = null;
                
                _logger.Debug("logout");
            }, RegistryAddresses.Logout);
        }

        private static IConfigurationRoot GetAppsettingsReader()
        {
            var builder = new ConfigurationBuilder()
                // .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            return configuration;
        }

        private static void ProcessWithPrinters()
        {
            List<string> GetPrinters()
            {
                var list = Printers.Get().ToList();
                _logger.Debug("Printers");
                list.ForEach(el => _logger.Debug(el));
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
        private static Client _client = null;
        private static void Subscriber(JobMeta job)
        {
            _logger.Verbose("job is here: " + job.DocumentName);
            if (CanContinueWithJob(job) == false)
                return;
            
            _jobs.Add(job.Guid);
            WriteLogs(job);
            CanchelIfNotValidJob(job);

            if (_client == null)
            {
                LaunchAuthGui().Wait();
                _client = Auth.GetClient();
            }

            var usrBalanceAllowPrinting = ClientValidator.CanContinuePrinting(_client, job);
            WriteLogsAboutJobAndClient(job, _client, usrBalanceAllowPrinting);
            if (!usrBalanceAllowPrinting)
            {
                _logger.Information("Job will be canceled");
                job._job.Cancel();
            }

            RunPrintJob(job._job);
            _jobs.Remove(job.Guid);
             //RemoveJobIfPaused(job, isPaused);
        }

        private static void CanchelIfNotValidJob(JobMeta job)
        {
            var jobIsValid = _validator.ApplyChecks(job);
            if (jobIsValid == false)
                job.CancelJob();
        }

        private static bool CanContinueWithJob(JobMeta job)
        {
            var isPaused = job._job.IsSpooling && job._job.IsPrinting == false
                           && job._job.IsPaused == true;
            var alreadyProcessedJob = _jobs.Contains(job.Guid) == true;
            return isPaused && alreadyProcessedJob == false;
        }

        private static async System.Threading.Tasks.Task LaunchAuthGui()
        {
            await GetResults();
            _logger.Information("Get AUTH after waiting!!!!!!");
        }
        
        private static async Task GetResults()
        {
            var startInfo = 
                new ProcessStartInfo(@"C:\Users\hopt\RiderProjects\PrinterManaging\DesctopGui\bin\Debug\netcoreapp3.1\DesctopGui.exe");
            Process.Start(startInfo);
            _logger.Information("Gui was started");
            continueClicked = new TaskCompletionSource<object>();
            await continueClicked.Task;
        }

        private static void WriteLogsAboutJobAndClient(JobMeta job, Client client, bool canBePrinted)
        {
            _logger.Information($"Client: {client.Email}, balance: {client.Balance}");
            _logger.Debug($"Job guid: {job.Guid}");
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
            _logger.Information("Everething ok, a print was started");
            _logger.Debug("___");
        }

        private static void WriteLogs(JobMeta job)
        {
            _logger.Information($"***********************************************");
            _logger.Information("Start job handler");
            _logger.Information($"Job state: {job._job.JobStatus.ToString()}");
            _logger.Information(job.Guid);
            _logger.Debug($"File to print: {job.DocumentName}");
            _logger.Debug($"Amount of pages: {job.CountOfPages}");
            _logger.Debug(
                $"Amount of copies: {job._job.HostingPrintQueue.CurrentJobSettings.CurrentPrintTicket.CopyCount}");
        }
    }
}