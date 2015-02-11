using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Infrastructure
{
    internal class Config : Interfaces.IConfig
    {
        public string StorageCdn { get { return ConfigurationManager.AppSettings["storagecdn"]; } }
    }
}