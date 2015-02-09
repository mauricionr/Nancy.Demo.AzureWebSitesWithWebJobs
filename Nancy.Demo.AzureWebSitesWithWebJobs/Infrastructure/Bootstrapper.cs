using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure
{
    public class Bootstrapper : Nancy.DefaultNancyBootstrapper
    {
        private readonly CloudTable _table;
        private readonly CloudBlobContainer _blobContainer;

        public Bootstrapper()
        {
            var storage = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["storage"].ConnectionString);

            // Blob
            var blobClient = storage.CreateCloudBlobClient();
            _blobContainer = blobClient.GetContainerReference("upload");

            // Table
            var tableClient = storage.CreateCloudTableClient();
            _table = tableClient.GetTableReference("storage");   
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            InitTableStorage();

            pipelines.BeforeRequest.AddItemToStartOfPipeline(Nancy.Security.SecurityHooks.RequiresHttps(true));
            container.Register<Repositories.ImageRepository>();
            container.Register(_blobContainer);
            container.Register(_table);

            base.ApplicationStartup(container, pipelines);
        }

        private void InitTableStorage()
        {
            _table.CreateIfNotExists();
            _blobContainer.CreateIfNotExists();
        }
    }
}