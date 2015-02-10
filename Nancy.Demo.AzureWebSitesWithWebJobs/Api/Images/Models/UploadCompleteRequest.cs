using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Api.Images.Models
{
    public class UploadCompleteRequest
    {
        public string StorageUrl { get; set; }
    }

    public class UploadCompleteRequestValidator : AbstractValidator<UploadCompleteRequest>
    {
        public UploadCompleteRequestValidator()
        {
            RuleFor(ucr => ucr.StorageUrl).NotNull().NotEmpty();
        }
    }
}