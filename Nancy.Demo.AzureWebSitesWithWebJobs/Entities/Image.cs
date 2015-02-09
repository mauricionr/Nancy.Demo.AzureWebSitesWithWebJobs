using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Entities
{
    public class Image : TableEntity
    {
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string Source { get; set; }
    }
}