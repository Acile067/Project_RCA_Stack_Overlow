using BCrypt.Net;
using StackOverflow.Domain.Contracts;
using StackOverflow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Application.Features.User.Commands.RegisterUser
{
    public class RegisterUserService
    {
        private readonly IRegisterRepository _registerRepository;
        public RegisterUserService(IRegisterRepository registerRepository)
        {
            _registerRepository = registerRepository;
        }
        public async Task<bool> RegisterUserAsync(RegisterUserDTO userDTO)
        {
            if (userDTO == null)
            {
                throw new ArgumentNullException(nameof(userDTO), "User data cannot be null");
            }
            if (string.IsNullOrEmpty(userDTO.Email) || string.IsNullOrEmpty(userDTO.Password))
            {
                throw new ArgumentException("Email and Password cannot be empty");
            }
            if (!await _registerRepository.IsEmailAvailableAsync(userDTO.Email))
            {
                throw new InvalidOperationException("Email is already registered");
            }
            if (string.IsNullOrEmpty(userDTO.Name) || string.IsNullOrEmpty(userDTO.LastName))
            {
                throw new ArgumentException("Name and Last Name cannot be empty");
            }
            if (string.IsNullOrEmpty(userDTO.Country) || string.IsNullOrEmpty(userDTO.Address) || string.IsNullOrEmpty(userDTO.City))
            {
                throw new ArgumentException("Country, Address, and City cannot be empty");
            }

            var id = Guid.NewGuid().ToString();
            var extension = GetExtensionFromContentType(userDTO.ProfilePictureContentType);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var cleanName = $"{userDTO.Name}_{userDTO.LastName}".ToLower().Replace(" ", "_");
            var filename = $"{cleanName}_{timestamp}_{id}{extension}";


            Domain.Entities.User user = new Domain.Entities.User(id)
            {
                Name = userDTO.Name,
                LastName = userDTO.LastName,
                Gender = userDTO.Gender,
                Country = userDTO.Country,
                Address = userDTO.Address,
                City = userDTO.City,
                Email = userDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                ProfilePictureFileName = filename,
                ProfilePictureContent = userDTO.ProfilePictureContent
            };


            return await _registerRepository.RegisterAsync(user);
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
