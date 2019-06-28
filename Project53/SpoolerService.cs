using System.ServiceProcess;
using Serilog;

namespace Project53
{
    public class SpoolerService
    {
        private ILogger _logger;

        public SpoolerService(ILogger logger) => _logger = logger;
        
        //ToDo It needs to be asynchronous.
        public void RunSpoolerService()
        {
            WindowsServiceManaging sc = new WindowsServiceManaging("Spooler");
            if (sc.Statuse != ServiceControllerStatus.Running)
                sc.StartService();

            while (sc.Statuse != ServiceControllerStatus.Running)
                ;

            _logger.Information("Print spoller running");
        }
    }
}