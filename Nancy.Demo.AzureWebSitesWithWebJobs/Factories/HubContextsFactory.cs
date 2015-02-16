using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Factories
{
    public class HubContextsFactory
    {
        public IHubContext GetContext()
        {
            var result = GlobalHost.ConnectionManager.GetHubContext<Hubs.ImagesHub>();
            return result;
        }
    }
}