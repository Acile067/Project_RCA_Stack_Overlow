using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Infrastructure.Common
{
    public class ProfilePictureBlobContext
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
        CloudBlobClient blobStorage;

        public ProfilePictureBlobContext()
        {
            blobStorage = storageAccount.CreateCloudBlobClient();
        }

        public async Task<bool> UploadImgAsync(string containerName, string fileName, byte[] fileContent)
        {
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(fileName) || fileContent == null || fileContent.Length == 0)
            {
                return false;
            }
            try
            {
                CloudBlobContainer container = blobStorage.GetContainerReference(containerName);
                await container.CreateIfNotExistsAsync();
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                using (var stream = new System.IO.MemoryStream(fileContent))
                {
                    await blockBlob.UploadFromStreamAsync(stream);
                }
                return true;
            }
            catch (StorageException ex)
            {
                // Log the exception (not implemented here)
                return false;
            }
        }
    }
}
