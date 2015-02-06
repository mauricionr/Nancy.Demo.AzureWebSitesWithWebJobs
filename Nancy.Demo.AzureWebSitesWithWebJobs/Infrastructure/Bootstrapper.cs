using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure
{
    public class Bootstrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(Nancy.Security.SecurityHooks.RequiresHttps(true));

            container.Register<Repositories.ImageRepository>();
            container.Register((c, n) =>
            {
                var storage = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["storage"].ConnectionString);
                var tableClient = storage.CreateCloudTableClient();
                tableClient.GetTableReference("storage").CreateIfNotExists();
                return tableClient;
            }).AsSingleton();

            base.ApplicationStartup(container, pipelines);
        }
    }
}