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
        public async Task<byte[]> DownloadImgAsync(string containerName, string fileName)
        {
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            try
            {
                CloudBlobContainer container = blobStorage.GetContainerReference(containerName);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                if (await blockBlob.ExistsAsync())
                {
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        await blockBlob.DownloadToStreamAsync(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
                return null;
            }
            catch (StorageException ex)
            {
                // Log the exception (not implemented here)
                return null;
            }
        }
        public async Task<bool> DeleteImgAsync(string containerName, string fileName)
        {
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            try
            {
                CloudBlobContainer container = blobStorage.GetContainerReference(containerName);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                return await blockBlob.DeleteIfExistsAsync();
            }
            catch (StorageException ex)
            {
                // Log the exception (not implemented here)
                return false;
            }
        }
    }
}
