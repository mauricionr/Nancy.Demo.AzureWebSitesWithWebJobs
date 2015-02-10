using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessor;
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
            [Blob("{Name}", FileAccess.Read)] Stream input,
            [Blob("handled/{BlobNameWithoutExtension}.jpg")] CloudBlockBlob outputBlob,
            [Blob("handled/{BlobNameWithoutExtension}_thumbnail.jpg")] CloudBlockBlob outputThumbnailBlob,
            [Table("images")]CloudTable imageTable,
            TextWriter log)
        {
            log.WriteLine("Got message from queue to process image: " + img.Source + ", with ID: " + img.RowKey);

            using (var output = outputThumbnailBlob.OpenWrite())
            {
                ProcessImage(input, output, quality: 70, maxWidth: 150);
            }
            using (var output = outputBlob.OpenWrite())
            {
                ProcessImage(input, output, quality: 100, maxWidth: 1920);
            }

            // Insert to table storage
            var imgEntry = new Image
            {
                PartitionKey = img.RowKey,
                RowKey = img.RowKey,
                Source = outputBlob.Uri.AbsolutePath,
                Thumbnail = outputThumbnailBlob.Uri.AbsolutePath
            };
            var opp = TableOperation.Insert(imgEntry);
            imageTable.Execute(opp);
        }

        private static void ProcessImage(Stream input, Stream output, int quality, int maxWidth)
        {
            var format = new PngFormat { Quality = quality };
            var maxSize = new Size(maxWidth, 0);
            using (var imageFactory = new ImageFactory(preserveExifData: true))
            {
                imageFactory.Load(input)
                            .Resize(maxSize)
                            .Format(format)
                            .Save(output);
            }
        }
    }
}
