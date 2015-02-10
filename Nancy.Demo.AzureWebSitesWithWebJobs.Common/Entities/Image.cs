using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Common.Entities
{
    public class Image : TableEntity
    {
        public string Id { get; set; }
        public string Thumbnail { get; set; }
        public string Source { get; set; }
    }
}