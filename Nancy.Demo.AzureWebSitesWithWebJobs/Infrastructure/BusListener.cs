using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.ServiceBus.Messaging;
using Nancy.Demo.AzureWebSitesWithWebJobs.Hubs;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure
{
    internal class BusListener
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly IHubContext _imageHub;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public BusListener(SubscriptionClient subscriptionClient, IHubContext imageHub)
        {
            if (subscriptionClient == null) throw new ArgumentNullException("subscriptionClient");
            if (imageHub == null) throw new ArgumentNullException("imageHub");

            _subscriptionClient = subscriptionClient;
            _imageHub = imageHub;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Init()
        {
            Task.Factory.StartNew(() => RunAsync());
        }

        private async Task RunAsync()
        {
            Debug.WriteLine("Bus listener started");
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var m = await _subscriptionClient.ReceiveAsync(TimeSpan.FromSeconds(5));
                    if (m != null)
                        InformClientsOfNewImage();
                    else
                        PingClients();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error receiving message from subscription " + e.ToString());
                }
            }
            Debug.WriteLine("Bus listener stopped");
        }

        private void PingClients()
        {
            Debug.WriteLine("Pinging clients...");
            _imageHub.Clients.All.ping();
            Debug.WriteLine("Clients pinged...");
        }

        private void InformClientsOfNewImage()
        {
            Debug.WriteLine("Informing clients of new image...");
            _imageHub.Clients.All.imageUploaded();
            Debug.WriteLine("Clients informed");
        }
    }
}