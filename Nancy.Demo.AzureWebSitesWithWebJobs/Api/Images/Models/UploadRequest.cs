using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Nancy.Validation.FluentValidation;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Api.Images.Models
{
    public class UploadRequest
    {
        public string Title { get; set; }
    }

    public class UploadRequestValidator : AbstractValidator<UploadRequest>
    {
        public UploadRequestValidator()
        {
            RuleFor(ur => ur.Title).NotNull();
        }
    }
}