using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using Microsoft.WindowsAzure.Storage.Table;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure
{
    public class Bootstrapper : Nancy.DefaultNancyBootstrapper
    {
        private readonly CloudTable _table;

        public Bootstrapper()
        {
            var storage = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["storage"].ConnectionString);
            var tableClient = storage.CreateCloudTableClient();
            _table = tableClient.GetTableReference("storage");   
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(Nancy.Security.SecurityHooks.RequiresHttps(true));
            InitTableStorage();
            container.Register<Repositories.ImageRepository>();
            container.Register(_table);

            base.ApplicationStartup(container, pipelines);
        }

        private void InitTableStorage()
        {
            _table.CreateIfNotExists();
        }
    }
}