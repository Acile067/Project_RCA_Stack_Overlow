using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.Contracts
{
    public interface IProfilePictureRepository
    {
        Task<byte[]> DownloadProfilePictureAsync(string fileName);
        Task<bool> UploadProfilePictureAsync(string fileName, byte[] fileContent);
        Task<bool> DeleteProfilePictureAsync(string fileName);
    }
}
