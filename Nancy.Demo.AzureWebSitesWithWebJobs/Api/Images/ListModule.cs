using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Nancy.Demo.AzureWebSitesWithWebJobs.Interfaces;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Api.Images
{
    public class ListModule : NancyModule
    {
        private readonly IImageRepository _imageRepository;

        public ListModule(IImageRepository imageRepository)
            : base("api/images/list/")
        {
            if (imageRepository == null) throw new ArgumentNullException("imageRepository");
            _imageRepository = imageRepository;

            Get["count", true] = GetImageCount;
            Get["{count:int}/{offset:int}", true] = GetImages;
        }

        private async Task<object> GetImageCount(object arg1, CancellationToken arg2)
        {
            var count = await _imageRepository.GetImageCountAsync();
            return new Models.ImageCountResponse
            {
                ImageCount = count
            };
        }

        private async Task<dynamic> GetImages(dynamic parameters, CancellationToken arg2)
        {
            if (parameters.offset == null)
                return HttpStatusCode.BadRequest;
            if (parameters.count == null)
                return HttpStatusCode.BadRequest;

            var count = parameters.count;
            var offset = parameters.offset;
            if (count <= 0 ||count > 12)
                return HttpStatusCode.BadRequest;
            if (offset < 0)
                return HttpStatusCode.BadRequest;

            var images = await _imageRepository.GetImagesAsync(count, offset);
            return images;
        }
    }
}