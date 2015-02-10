using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Common.Entities
{
    public class Image : TableEntity
    {
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string Source { get; set; }

        public Uri Url
        {
            get
            {
                Uri uri;
                if (string.IsNullOrEmpty(Source) || !Uri.TryCreate(Source, UriKind.Absolute, out uri))
                    return null;
                return uri;
            }
        }

        public string Path
        {
            get
            {
                if (Url == null)
                    return null;
                return Url.AbsolutePath;
            }
        }

        public string Name
        {
            get
            {
                if (Url == null)
                    return null;
                return Url.Segments[Url.Segments.Length - 1];
            }
        }

        public string NameWithoutExtension
        {
            get
            {
                if (Url == null)
                    return null;
                return System.IO.Path.GetFileNameWithoutExtension(Name);
            }
        } 
    }
}