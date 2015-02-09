using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Repositories
{
    public class ImageRepository : Interfaces.IImageRepository
    {
        private const int TotalCount = 241;
        private readonly CloudTable _table;

        public ImageRepository(Microsoft.WindowsAzure.Storage.Table.CloudTable table)
        {
            if (table == null) throw new ArgumentNullException("table");
            _table = table;
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
    }
}