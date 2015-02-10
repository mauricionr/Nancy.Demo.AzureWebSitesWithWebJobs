using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Nancy.Demo.AzureWebSitesWithWebJobs.Common.Entities;
using Image = Nancy.Demo.AzureWebSitesWithWebJobs.Common.Entities.Image;

namespace Nancy.Demo.AzureWebSitesWithWebJobs.Jo
{
    public class Functions
    {
        public static void ProcessImageQueueMessage(
            [QueueTrigger("images")] Common.Entities.Image img,
            [Blob("images/uploads/{Id}", FileAccess.Read)] Stream input,
            [Blob("images/handled/{Id}.png")] CloudBlockBlob outputBlob,
            [Blob("images/handled/{Id}_thumbnail.png")] CloudBlockBlob outputThumbnailBlob,
            [Table("images")]CloudTable imageTable,
            TextWriter log)
        {
            log.WriteLine("Got message from queue to process image: " + img.Source + ", with ID: " + img.RowKey);

            using (var output = outputThumbnailBlob.OpenWrite())
            {
                ProcessImage(input, output, quality: 70, maxWidth: 150);
            }
            log.WriteLine("Created thumbnail for " + img.Id);
            using (var output = outputBlob.OpenWrite())
            {
                ProcessImage(input, output, quality: 100, maxWidth: 1920);
            }
            log.WriteLine("Created full image for " + img.Id);

            // Insert to table storage
            img.Source = outputBlob.Uri.AbsolutePath;
            img.Thumbnail = outputThumbnailBlob.Uri.AbsolutePath;

            var opp = TableOperation.Insert(img);
            imageTable.Execute(opp);
            log.WriteLine("Inserted outputted image for " + img.Id);
        }

        private static void ProcessImage(Stream input, Stream output, int quality, int maxWidth)
        {
            var format = new PngFormat { Quality = quality };
            var maxSize = new Size(maxWidth, 0);
            var resize = new ResizeLayer(maxSize, ResizeMode.Max, AnchorPosition.Center, false, null, maxSize);
            using (var memoryOutput = new MemoryStream())
            {
                using (var imageFactory = new ImageFactory(preserveExifData: true))
                {
                    imageFactory.Load(input)
                                .Resize(resize)
                                .Format(format)
                                .Save(memoryOutput);
                }
                memoryOutput.Seek(0, SeekOrigin.Begin);
                memoryOutput.CopyTo(output);
            }
        }
    }
}
