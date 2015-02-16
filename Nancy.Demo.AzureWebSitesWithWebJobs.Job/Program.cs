using System.Configuration;
using Microsoft.Azure.WebJobs;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Job
{
    class Program
    {
        static void Main()
        {
            var jobHostConfiguration = new JobHostConfiguration
            {
                ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["servicebus"].ConnectionString
            };
            var host = new JobHost(jobHostConfiguration);
            host.RunAndBlock();
        }
    }
}
