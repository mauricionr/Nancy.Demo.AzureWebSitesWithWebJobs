using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.ServiceBus.Messaging;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Hubs
{
    public class ImagesHub : Hub
    {
        public void InformClientsOfNewImage()
        {
            Clients.All.imageUploaded();
        }
    }
}