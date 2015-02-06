using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Repositories
{
    public class ImageRepository : Interfaces.IImageRepository
    {
        public ImageRepository()
        {
        }

        public Task<IReadOnlyCollection<Models.Image>> GetImagesAsync(int count, int offset)
        {
            var totalCount = 200;
            var images = new List<Models.Image>(totalCount);
            for (var i = 0; i < totalCount; i++)
            {
                var img = new Models.Image("Id-" + i, "Title " + i, "//placehold.it/350x150", "//placehold.it/1024x768");
                images.Add(img);
            }

            var result = images.Skip(offset).Take(count).ToList();
            return Task.FromResult<IReadOnlyCollection<Models.Image>>(result);
        }
    }
}