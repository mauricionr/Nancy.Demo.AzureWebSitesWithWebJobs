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

            Post["", true] = UploadImage;
        }

        private async Task<object> UploadImage(object parameters, CancellationToken arg2)
        {
            var request = this.BindAndValidate<Models.UploadRequest>();
            if (!ModelValidationResult.IsValid)
                return HttpStatusCode.BadRequest;
            var file = Request.Files.FirstOrDefault();
            if (file == null)
                return HttpStatusCode.BadRequest;

            await _imageRepository.UploadImageAsync(request.Title, file.Value);
            
            return HttpStatusCode.NoContent;
        }
    }
}