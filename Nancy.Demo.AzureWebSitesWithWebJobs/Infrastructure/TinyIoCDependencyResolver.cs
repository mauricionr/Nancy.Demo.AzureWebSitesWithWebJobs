using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Nancy.TinyIoc;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure
{
    public class TinyIoCDependencyResolver : DefaultDependencyResolver
    {
        private readonly TinyIoCContainer _container;

        public TinyIoCDependencyResolver(TinyIoCContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            var result = _container.CanResolve(serviceType) ? _container.Resolve(serviceType) : base.GetService(serviceType);
            return result;
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var objects = _container.CanResolve(serviceType) ? _container.ResolveAll(serviceType) : new object[] { };
            var result = objects.Concat(base.GetServices(serviceType));
            return result;
        }
    }
}