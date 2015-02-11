using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Api.Images.Models
{
    public class DeleteImageRequest
    {
        public string ImageId { get; set; }
    }

    public class DeleteImageRequestValidator : AbstractValidator<DeleteImageRequest>
    {
        public DeleteImageRequestValidator()
        {
            RuleFor(dir => dir.ImageId).NotNull().NotEmpty();
        }
    }
}