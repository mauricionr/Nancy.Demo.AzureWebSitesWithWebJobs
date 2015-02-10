using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Repositories
{
    public class ImageRepository : Interfaces.IImageRepository
    {
        private const int TotalCount = 241;
        private readonly CloudTable _table;
        private readonly CloudBlobContainer _blobContainer;
        private readonly CloudQueue _queue;

        public string StorageDomain { get { return _blobContainer.Uri.Host; } }

        public ImageRepository(CloudTable table, Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer blobContainer, Microsoft.WindowsAzure.Storage.Queue.CloudQueue queue)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (blobContainer == null) throw new ArgumentNullException("blobContainer");
            if (queue == null) throw new ArgumentNullException("queue");
            _table = table;
            _blobContainer = blobContainer;
            _queue = queue;
        }

        public Task<IReadOnlyCollection<Models.Image>> GetImagesAsync(int count, int offset)
        {
            var images = new List<Models.Image>(TotalCount);
            var rnd = new Random();
            for (var i = 0; i < TotalCount; i++)
            {
                var thumbnailHeight = rnd.Next(300, 350);
                var thumbnailWidth = rnd.Next(150, 200);
                var height = rnd.Next(1024, 1920);
                var width = rnd.Next(768, 1440);
                var img = new Models.Image("Id-" + i, "Title " + i, "//placehold.it/" + thumbnailWidth + "x" + thumbnailHeight, "//placehold.it/" + width + "x" + height);
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
            var img = new Common.Entities.Image { PartitionKey = "uploaded", RowKey = id, Id = title, Source = path };
            var operation = TableOperation.Insert(img);
            var insertTask = _table.ExecuteAsync(operation);

            await Task.WhenAll(uploadTask, insertTask);
        }

        public Task<string> GetImageUploadUrlAsync()
        {
            var id = Guid.NewGuid().ToString("N");
            var path = "uploads/" + id;
            var blob = _blobContainer.GetBlockBlobReference(path);
            var sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(4),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write
            };

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            var sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);
            var url = blob.Uri.AbsoluteUri + sasBlobToken;
            return Task.FromResult(url);
        }

        public async Task ImageUploadCompleteAsync(string storageUrl)
        {
            if (string.IsNullOrEmpty(storageUrl))
                throw new ArgumentNullException("storageUrl");

            var uri = new Uri(storageUrl);
            var path = uri.AbsolutePath;
            var img = new Common.Entities.Image { PartitionKey = "uploaded", RowKey = Guid.NewGuid().ToString("N"), Source = path };
            var message = JsonConvert.SerializeObject(img);
            var queueMessage = new CloudQueueMessage(message);
            await _queue.AddMessageAsync(queueMessage);
        }
    }
}