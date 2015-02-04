using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
            : base("/")
        {
            Get[""] = _ => View["Home"];
        }
    }
}