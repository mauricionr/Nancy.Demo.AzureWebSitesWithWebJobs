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
using Nancy.Demo.AzureWebSitesWithWebJobs.Factories;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure
{
    internal class BusListener
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly HubContextsFactory _hubContextsFactory;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public BusListener(SubscriptionClient subscriptionClient, Factories.HubContextsFactory hubContextsFactory)
        {
            if (subscriptionClient == null) throw new ArgumentNullException("subscriptionClient");
            if (hubContextsFactory == null) throw new ArgumentNullException("hubContextsFactory");

            _subscriptionClient = subscriptionClient;
            _hubContextsFactory = hubContextsFactory;
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
            var hub = _hubContextsFactory.GetContext();
            hub.Clients.All.ping();
            Debug.WriteLine("Clients pinged...");
        }

        private void InformClientsOfNewImage()
        {
            Debug.WriteLine("Informing clients of new image...");
            var hub = _hubContextsFactory.GetContext();
            hub.Clients.All.imageUploaded();
            Debug.WriteLine("Clients informed");
        }
    }
}