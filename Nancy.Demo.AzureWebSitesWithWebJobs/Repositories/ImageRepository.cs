﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Repositories
{
    public class ImageRepository : Interfaces.IImageRepository
    {
        private const int TotalCount = 241;
        private readonly CloudTable _table;
        private readonly CloudBlobContainer _blobContainer;

        public ImageRepository(CloudTable table, Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer blobContainer)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (blobContainer == null) throw new ArgumentNullException("blobContainer");
            _table = table;
            _blobContainer = blobContainer;
        }

        public Task<IReadOnlyCollection<Models.Image>> GetImagesAsync(int count, int offset)
        {
            var images = new List<Models.Image>(TotalCount);
            for (var i = 0; i < TotalCount; i++)
            {
                var img = new Models.Image("Id-" + i, "Title " + i, "//placehold.it/350x150", "//placehold.it/1024x768");
                images.Add(img);
            }

            var result = images.Skip(offset).Take(count).ToList();
            return Task.FromResult<IReadOnlyCollection<Models.Image>>(result);
        }

        public Task<int> GetImageCountAsync()
        {
            return Task.FromResult(TotalCount);
        }

        public async Task UploadImageAsync(string title, Stream value)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException("title");
            if (value == null) throw new ArgumentNullException("value");

            // upload image to blob storage
            var id = Guid.NewGuid().ToString("N");
            var path = "uploaded/" + id;
            var blob = _blobContainer.GetBlockBlobReference(path);
            var uploadTask = blob.UploadFromStreamAsync(value);

            // add image to table storage
            var img = new Entities.Image { PartitionKey = "uploaded", RowKey = id, Title = title, Source = path };
            var operation = TableOperation.Insert(img);
            var insertTask = _table.ExecuteAsync(operation);

            await Task.WhenAll(uploadTask, insertTask);
        }
    }
}