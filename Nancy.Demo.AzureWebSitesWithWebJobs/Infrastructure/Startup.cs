using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure;
using Nancy.Owin;
using Nancy.TinyIoc;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var serviceBusScaleoutConfiguration = new ServiceBusScaleoutConfiguration(ConfigurationManager.ConnectionStrings["servicebus"].ConnectionString, "nancysignals")
            {
                BackoffTime = TimeSpan.FromSeconds(5)
            };

            var ioc = new TinyIoCContainer();
            var bootstrapper = new Bootstrapper(ioc);
            var hubConfiguration = new HubConfiguration
            {
                Resolver = bootstrapper.DependencyResolver
            };
            hubConfiguration.Resolver.UseServiceBus(serviceBusScaleoutConfiguration);
            var nancyOptions = new NancyOptions
            {
                Bootstrapper = bootstrapper
            };

            GlobalHost.DependencyResolver = bootstrapper.DependencyResolver;

            app.MapSignalR(hubConfiguration);
            app.UseNancy(nancyOptions);
        }
    }
}