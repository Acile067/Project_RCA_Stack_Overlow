using StackOverflow.Domain.Contracts;
using StackOverflow.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Infrastructure.Repository
{
    public class ProfilePictureRepository : IProfilePictureRepository
    {
        private readonly ProfilePictureBlobContext _blobContext;
        public ProfilePictureRepository(ProfilePictureBlobContext blobContext)
        {
            _blobContext = blobContext ?? throw new ArgumentNullException(nameof(blobContext));
        }

        public async Task<byte[]> DownloadProfilePictureAsync(string fileName)
        {
            
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            try
            {
                // Assuming the container name is "profilepictures"
                return await _blobContext.DownloadImgAsync("profilepictures", fileName);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return null;
            }
        }
    }
}
