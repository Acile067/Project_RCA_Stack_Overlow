using StackOverflow.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfilePictureRepository _profilePictureRepository;
        public UpdateUserService(IUserRepository userRepository, IProfilePictureRepository profilePictureRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _profilePictureRepository = profilePictureRepository ?? throw new ArgumentNullException(nameof(profilePictureRepository));
        }
        public async Task<bool> UpdateUserAsync(UpdateUserDTO updateUserDTO)
        {
            if (updateUserDTO == null)
            {
                throw new ArgumentNullException(nameof(updateUserDTO), "UpdateUserDTO cannot be null.");
            }
            if (string.IsNullOrEmpty(updateUserDTO.Id))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(updateUserDTO.Id));
            }
            var user = await _userRepository.GetUserByIdAsync(updateUserDTO.Id);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {updateUserDTO.Id} not found.");
            }

            

            if (user.ProfilePictureFileName != updateUserDTO.ProfilePictureFileName)
            {
                var id = Guid.NewGuid().ToString();
                var extension = GetExtensionFromContentType(updateUserDTO.ProfilePictureContentType);
                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var cleanName = $"{updateUserDTO.Name}_{updateUserDTO.LastName}".ToLower().Replace(" ", "_");
                var filename = $"{cleanName}_{timestamp}_{id}{extension}";

                if (!string.IsNullOrEmpty(updateUserDTO.OldProfilePictureFileName) && !string.IsNullOrEmpty(updateUserDTO.ProfilePictureFileName))
                {
                    await _profilePictureRepository.DeleteProfilePictureAsync(updateUserDTO.OldProfilePictureFileName);
                    await _profilePictureRepository.UploadProfilePictureAsync(
                        filename,
                        updateUserDTO.ProfilePictureContent
                    );
                

                user.ProfilePictureFileName = updateUserDTO.ProfilePictureFileName;
                user.ProfilePictureContent = updateUserDTO.ProfilePictureContent;
                }
            }

            user.Name = updateUserDTO.Name;
            user.LastName = updateUserDTO.LastName;
            user.Gender = updateUserDTO.Gender;
            user.Email = updateUserDTO.Email;
            user.Country = updateUserDTO.Country;
            user.Address = updateUserDTO.Address;
            user.City = updateUserDTO.City;

            if (!string.IsNullOrEmpty(updateUserDTO.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(updateUserDTO.Password);
            }
           
            user.LastUpdatedAt = DateTime.UtcNow;

            var result = await _userRepository.UpdateUserAsync(user);

            return result;
        }
        private string GetExtensionFromContentType(string contentType)
        {
            if (contentType == "image/jpeg")
                return ".jpg";
            else if (contentType == "image/png")
                return ".png";
            else if (contentType == "image/gif")
                return ".gif";
            else if (contentType == "image/webp")
                return ".webp";
            else
                return ".bin";
        }
    }
}
