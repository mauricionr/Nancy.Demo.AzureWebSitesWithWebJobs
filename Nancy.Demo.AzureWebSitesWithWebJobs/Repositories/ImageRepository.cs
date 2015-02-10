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
        private readonly CloudTable _table;
        private readonly CloudBlobContainer _blobContainer;
        private readonly CloudQueue _queue;

        public ImageRepository(CloudTable table, Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer blobContainer, Microsoft.WindowsAzure.Storage.Queue.CloudQueue queue)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (blobContainer == null) throw new ArgumentNullException("blobContainer");
            if (queue == null) throw new ArgumentNullException("queue");
            _table = table;
            _blobContainer = blobContainer;
            _queue = queue;
        }

        public async Task<IReadOnlyCollection<Models.Image>> GetImagesAsync(int count, int offset)
        {
            var urlPrefix = "https://" + _blobContainer.Uri.Host;
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

        public async Task ImageUploadCompleteAsync(string storageUrl)
        {
            if (string.IsNullOrEmpty(storageUrl))
                throw new ArgumentNullException("storageUrl");

            var uri = new Uri(storageUrl);
            var id = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
            var img = new Common.Entities.Image { PartitionKey = Guid.NewGuid().ToString("N"), RowKey = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), Id = id };
            var message = JsonConvert.SerializeObject(img);
            var queueMessage = new CloudQueueMessage(message);
            await _queue.AddMessageAsync(queueMessage);
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