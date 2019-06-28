using System;
using System.ServiceProcess;
using Serilog;

namespace Project53
{
    public class WindowsServiceManaging
    {
        private static ILogger _logger;// = LogManager.GetCurrentClassLogger();
        private ServiceController _scController;

        public WindowsServiceManaging(string serviceName)
        {
            _logger.Information($"ServiceController inited for {serviceName}");
            _scController = new ServiceController(serviceName);
            _scController.MachineName = Environment.MachineName;
        }

        public ServiceControllerStatus Statuse
        {
            get
            {
                _logger.Information($"Trying statuse.");
                _scController.Refresh();
                _logger.Information($"Statuse {_scController.Status}");
                return _scController.Status;
            }
        }

        public void Stop()
        {
            try
            {
                _scController.Stop();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occured while the service stopped");
            }
        }

        public void StartService()
        {
            try
            {
                _scController.MachineName = Environment.MachineName;
                _scController.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "");
            }
        }
    }
}