using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using Microsoft.AspNet.SignalR;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure
{
    public class Bootstrapper : Nancy.DefaultNancyBootstrapper
    {
        private readonly TinyIoCContainer _container;
        private readonly TinyIoCDependencyResolver _dependencyResolver;
        private readonly CloudBlobClient _blobClient;
        private readonly CloudTable _table;
        private readonly CloudBlobContainer _blobContainer;
        private readonly CloudQueue _queue;
        private readonly NamespaceManager _namespaceManager;
        private readonly SubscriptionClient _subscriptionClient;
        private BusListener _busListener;

        public TinyIoCDependencyResolver DependencyResolver { get { return _dependencyResolver; } }

        public Bootstrapper(TinyIoCContainer ioc)
        {
            _container = ioc;
            _dependencyResolver = new TinyIoCDependencyResolver(_container);

            // service bus
            _namespaceManager = NamespaceManager.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["servicebus"].ConnectionString);
            _subscriptionClient = SubscriptionClient.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["servicebus"].ConnectionString, "images", "nancysubscription", ReceiveMode.ReceiveAndDelete);
            
            // storage
            var storage = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["storage"].ConnectionString);

            // Blob
            _blobClient = storage.CreateCloudBlobClient();
            _blobContainer = _blobClient.GetContainerReference("images");

            // Table
            var tableClient = storage.CreateCloudTableClient();
            _table = tableClient.GetTableReference("images");

            // queue
            var queueClient = storage.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("images");
        }

        protected override TinyIoCContainer GetApplicationContainer()
        {
            return _container;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.Register<Repositories.ImageRepository>();
            container.Register<Infrastructure.Config>();
            container.Register(_blobContainer);
            container.Register(_table);
            container.Register(_queue);
            container.Register(_subscriptionClient);
            container.Register((c, p) =>
            {
                var result = GlobalHost.ConnectionManager.GetHubContext<Hubs.ImagesHub>();
                return result;
            });

            InitTableStorage();
            InitServiceBus();
#if !DEBUG
            pipelines.BeforeRequest.AddItemToStartOfPipeline(Nancy.Security.SecurityHooks.RequiresHttps(true));
#endif

            pipelines.AfterRequest += PostRequest;

            _busListener = _container.Resolve<Infrastructure.BusListener>();
            _busListener.Init();
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("signalr"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("App"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Img"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Content"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts"));
        }

        private void PostRequest(NancyContext ctx)
        {
            ctx.Response.WithHeader("Access-Control-Allow-Origin", ctx.Request.Url.Scheme + "://" + _blobContainer.Uri.Host);
        }

        private void InitTableStorage()
        {
            var blobProperties = _blobClient.GetServiceProperties();
            ConfigureCors(blobProperties);
            _blobClient.SetServiceProperties(blobProperties);

            _table.CreateIfNotExists();
            _blobContainer.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
            _queue.CreateIfNotExists();
        }

        private static void ConfigureCors(ServiceProperties serviceProperties)
        {
            serviceProperties.Cors = new CorsProperties();
            serviceProperties.Cors.CorsRules.Add(new CorsRule
            {
                AllowedHeaders = new List<string>() { "*" },
                AllowedMethods = CorsHttpMethods.Put | CorsHttpMethods.Get | CorsHttpMethods.Head | CorsHttpMethods.Post,
                AllowedOrigins = new List<string>() { "*" },
                ExposedHeaders = new List<string>() { "*" },
                MaxAgeInSeconds = 1800 // 30 minutes
            });
        }

        private void InitServiceBus()
        {
            if (!_namespaceManager.TopicExists("images"))
                _namespaceManager.CreateTopic("images");
            if (!_namespaceManager.SubscriptionExists("images", "nancysubscription"))
                _namespaceManager.CreateSubscription("images", "nancysubscription");
        }
    }
}