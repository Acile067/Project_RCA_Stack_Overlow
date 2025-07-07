using StackOverflow.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Application.Features.User.Quieres.ProfilePictureQuiery
{
    public class UserProfilePictureService
    {
        private readonly IProfilePictureRepository _profilePictureRepository;
        public UserProfilePictureService(IProfilePictureRepository profilePictureRepository)
        {
            _profilePictureRepository = profilePictureRepository ?? throw new ArgumentNullException(nameof(profilePictureRepository));
        }
        public async Task<byte[]> GetProfilePictureAsync(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("filename cannot be null or empty.", nameof(filename));
            }
            try
            {
                return await _profilePictureRepository.DownloadProfilePictureAsync(filename);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while retrieving the profile picture.", ex);
            }
        }
    }
}
