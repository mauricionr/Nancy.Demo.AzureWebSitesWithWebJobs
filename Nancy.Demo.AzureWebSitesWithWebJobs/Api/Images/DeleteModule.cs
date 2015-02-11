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
    public class DeleteModule : NancyModule
    {
        private readonly IImageRepository _imageRepository;

        public DeleteModule(Interfaces.IImageRepository imageRepository) : base("api/images/delete")
        {
            if (imageRepository == null) throw new ArgumentNullException("imageRepository");
            _imageRepository = imageRepository;

            Post["", true] = DeleteImage;
        }

        private async Task<object> DeleteImage(object arg1, CancellationToken arg2)
        {
            var request = this.BindAndValidate<Models.DeleteImageRequest>();
            if (!ModelValidationResult.IsValid)
                return HttpStatusCode.BadRequest;

            await _imageRepository.DeleteImageAsync(request.ImageId);

            return HttpStatusCode.NoContent;
        }
    }
}