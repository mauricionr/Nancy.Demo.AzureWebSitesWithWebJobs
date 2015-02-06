using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Models
{
    public class Image
    {
        private readonly string _id;
        private readonly string _title;
        private readonly string _thumbnail;
        private readonly string _source;

        public string Id { get { return _id; } }
        public string Title { get { return _title; } }
        public string Thumbnail { get { return _thumbnail; } }
        public string Source { get { return _source; } }

        public Image(string id, string title, string thumbnail, string source)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException("title");
            if (string.IsNullOrEmpty(thumbnail))
                throw new ArgumentNullException("thumbnail");
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            _id = id;
            _title = title;
            _thumbnail = thumbnail;
            _source = source;
        }
    }
}