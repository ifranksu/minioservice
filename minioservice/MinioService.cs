using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Topshelf;
using Topshelf.Logging;
using System.Configuration;

namespace minioservice
{
    public class MinioService : ServiceControl
    {
        private readonly LogWriter _log = HostLogger.Get<MinioService>();

        private string _minioPath = ConfigurationManager.AppSettings[Constants.MINIO_PAHT];
        private string _storagePath = ConfigurationManager.AppSettings[Constants.MINIO_STORAGE_PAHT];
        private string _minioServer;
        private Process _minioServerConsole;

        bool VerifyArgumnets()
        {
            if (!Directory.Exists(_minioPath))
            {
                _log.ErrorFormat("minio directory not found->{0}", _minioPath);
                return false;
            }
            _minioServer = Path.Combine(_minioPath, ConfigurationManager.AppSettings[Constants.MINIO_SERVER]);

            return true;
        }

        public bool Start(HostControl hostControl)
        {
            if (!VerifyArgumnets()) return false;

            _log.Info("MinioService Starting...");

            try
            {
                ProcessStartInfo cfg = new ProcessStartInfo(_minioServer)
                {
                    UseShellExecute = false,
                    Arguments = string.Format(" --config-dir {0}\\.minio server {1}", _minioPath, _storagePath),
                    WorkingDirectory = _minioPath
                };

                using (_minioServerConsole = new Process { StartInfo = cfg })
                {
                    _minioServerConsole.Start();
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Starting minio console server occured exception:{0}", ex);
                return false;
            }

            _log.Info("MinioService Started");

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            if (_minioServerConsole != null)
            {
                try
                {
                    foreach (var process in Process.GetProcessesByName("minio"))
                    {
                        process.Kill();
                    }
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("close server occured exception:{0}", ex);
                    return false;
                }
            }

            Thread.Sleep(500);

            _log.Info("MinioService Stopped");

            return true;
        }
    }
}
