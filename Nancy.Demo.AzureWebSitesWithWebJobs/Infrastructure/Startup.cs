using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
        }
    }
}