using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Interfaces
{
    public interface IImageRepository
    {
        Task<IReadOnlyCollection<Models.Image>> GetImagesAsync(int count, int offset);
        Task<int> GetImageCountAsync();
        Task UploadImageAsync(string title, Stream value);
        Task<string> GetImageUploadUrlAsync();
        string StorageDomain { get; }
        Task ImageUploadCompleteAsync(string storageUrl);
    }
}
