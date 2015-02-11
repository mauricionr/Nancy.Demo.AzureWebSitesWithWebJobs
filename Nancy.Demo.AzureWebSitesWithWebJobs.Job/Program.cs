using Microsoft.Azure.WebJobs;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Job
{
    class Program
    {
        static void Main()
        {
            var host = new JobHost();
            host.RunAndBlock();
        }
    }
}
