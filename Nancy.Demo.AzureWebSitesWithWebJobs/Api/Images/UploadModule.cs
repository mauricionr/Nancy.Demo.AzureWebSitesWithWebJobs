using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Nancy.Demo.AzureWebSitesWithWebJobs.Interfaces;
using Nancy.ModelBinding;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Api.Images
{
    public class UploadModule : NancyModule
    {
        private readonly IImageRepository _imageRepository;

        public UploadModule(Interfaces.IImageRepository imageRepository)
            : base("api/images/upload/")
        {
            if (imageRepository == null) throw new ArgumentNullException("imageRepository");
            _imageRepository = imageRepository;

            Get["url", true] = GetImageUploadUrl;
            Post["complete", true] = ImageUploadComplete;
        }

        private async Task<object> ImageUploadComplete(object arg1, CancellationToken arg2)
        {
            var request = this.BindAndValidate<Models.UploadCompleteRequest>();
            if (!ModelValidationResult.IsValid)
                return HttpStatusCode.BadRequest;

            await _imageRepository.ImageUploadCompleteAsync(request.ContentType, request.StorageUrl);
            return HttpStatusCode.NoContent;
        }

        private async Task<object> GetImageUploadUrl(object arg1, CancellationToken arg2)
        {
            var url = await _imageRepository.GetImageUploadUrlAsync();
            return url;
        }
    }
}