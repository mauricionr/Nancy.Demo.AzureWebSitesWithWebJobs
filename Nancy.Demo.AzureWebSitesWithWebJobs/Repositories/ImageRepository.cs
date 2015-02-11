using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Nancy.Demo.AzureWebSitesWithWebJobs.Interfaces;
using Newtonsoft.Json;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Repositories
{
    public class ImageRepository : Interfaces.IImageRepository
    {
        private readonly CloudTable _table;
        private readonly CloudBlobContainer _blobContainer;
        private readonly CloudQueue _queue;
        private readonly IConfig _config;

        public ImageRepository(CloudTable table, Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer blobContainer, Microsoft.WindowsAzure.Storage.Queue.CloudQueue queue, Interfaces.IConfig config)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (blobContainer == null) throw new ArgumentNullException("blobContainer");
            if (queue == null) throw new ArgumentNullException("queue");
            if (config == null) throw new ArgumentNullException("config");
            _table = table;
            _blobContainer = blobContainer;
            _queue = queue;
            _config = config;
        }

        public async Task<IReadOnlyCollection<Models.Image>> GetImagesAsync(int count, int offset)
        {
            var urlPrefix = "//" + _blobContainer.Uri.Host;
            if (!string.IsNullOrEmpty(_config.StorageCdn))
                urlPrefix = "//" + _config.StorageCdn;
            var list = await GetImageList();
            var result = list.Skip(offset).Take(count).Select(i => new Models.Image(i.Id, i.Id, urlPrefix + i.Thumbnail, urlPrefix + i.Source)).ToList();
            return result;
        }

        public async Task<int> GetImageCountAsync()
        {
            var list = await GetImageList();
            return list.Count;
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

        public async Task ImageUploadCompleteAsync(string contentType, string storageUrl)
        {
            if (string.IsNullOrEmpty(contentType))
                throw new ArgumentNullException("contentType");
            if (string.IsNullOrEmpty(storageUrl))
                throw new ArgumentNullException("storageUrl");

            var uri = new Uri(storageUrl);
            var id = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
            var img = new Common.Entities.Image { PartitionKey = Guid.NewGuid().ToString("N"), RowKey = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), Id = id, ContentType = contentType };
            var message = JsonConvert.SerializeObject(img);
            var queueMessage = new CloudQueueMessage(message);
            await _queue.AddMessageAsync(queueMessage);
        }

        public async Task DeleteImageAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            var images = await GetImageList();
            var img = images.FirstOrDefault(i => i.Id == id);
            if (img == null)
                return;

            var source = img.Source.Substring(_blobContainer.Name.Length + 2);
            var blob = _blobContainer.GetBlockBlobReference(source);
            await blob.DeleteIfExistsAsync();
            source = img.Thumbnail.Substring(_blobContainer.Name.Length + 2);
            blob = _blobContainer.GetBlockBlobReference(source);
            await blob.DeleteIfExistsAsync();

            var opp = TableOperation.Delete(img);
            await _table.ExecuteAsync(opp);
        }

        private async Task<IReadOnlyCollection<Common.Entities.Image>> GetImageList()
        {
            var query = new TableQuery<Common.Entities.Image>();
            var response = await _table.ExecuteQuerySegmentedAsync(query, null);
            var list = new List<Common.Entities.Image>();
            list.AddRange(response.Results);
            while (response.ContinuationToken != null)
            {
                response = await _table.ExecuteQuerySegmentedAsync(query, response.ContinuationToken);
                list.AddRange(response.Results);
            }
            return list;
        }
    }
}