using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StackOverflowService.Repositories
{
    public class ProfilePictureBlobService
    {
        private readonly CloudBlobContainer _container;

        public ProfilePictureBlobService()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference("profilepictures");
            _container.CreateIfNotExists();
        }

        public async Task<string> UploadFileAsync(HttpPostedFile file)
        {
            var blob = _container.GetBlockBlobReference(Guid.NewGuid() + Path.GetExtension(file.FileName));
            await blob.UploadFromStreamAsync(file.InputStream);
            return blob.Uri.ToString();
        }
    }
}