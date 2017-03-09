using System.Configuration;
using Topshelf;

namespace minioservice
{
    class Program
    {
        static string _ServiceName = ConfigurationManager.AppSettings[Constants.MINIO_SERVICE_NAME];
        static string _ServiceDisplayName = ConfigurationManager.AppSettings[Constants.MINIO_SERVICE_DISPALYNAME];
        static string _ServiceDescription = ConfigurationManager.AppSettings[Constants.MINIO_SERVICE_DESCRIPTION];

        static int Main(string[] args)
        {
            return (int)HostFactory.Run(x =>
            {
                x.UseLog4Net("log4net.config");

                x.RunAsLocalSystem();
                x.SetServiceName(_ServiceName);
                x.SetDisplayName(_ServiceDisplayName);
                x.SetDescription(_ServiceDescription);

                x.Service<MinioService>();

                x.EnableServiceRecovery(r => { r.RestartService(1); });
            });
        }
    }
}
