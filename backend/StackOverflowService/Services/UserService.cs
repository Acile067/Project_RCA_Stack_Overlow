using StackOverflowService.DTOs;
using StackOverflowService.Entities;
using StackOverflowService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace StackOverflowService.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepo;
        private readonly ProfilePictureBlobService _profilePictureBlobService;

        public UserService(UserRepository userRepo, ProfilePictureBlobService profilePictureBlobService)
        {
            _userRepo = userRepo;
            _profilePictureBlobService = profilePictureBlobService;
        }

        public async Task<ValidationResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            var errors = new List<FieldError>();

            // Validacije polja
            if (string.IsNullOrWhiteSpace(dto.FullName))
                errors.Add(new FieldError { Field = "fullName", Message = "Full name is required." });

            if (string.IsNullOrWhiteSpace(dto.Email))
                errors.Add(new FieldError { Field = "email", Message = "Email is required." });
            else if (!dto.Email.Contains("@"))
                errors.Add(new FieldError { Field = "email", Message = "Invalid email format." });

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                errors.Add(new FieldError { Field = "password", Message = "Password must be at least 6 characters." });

            if (dto.ProfileImage == null)
                errors.Add(new FieldError { Field = "profileImage", Message = "Profile image is required." });

            // Ako ima validacionih grešaka
            if (errors.Any())
            {
                return new ValidationResponseDto
                {
                    Success = false,
                    Errors = errors
                };
            }

            // Provera duplikata email-a
            if (await _userRepo.EmailExistsAsync(dto.Email))
            {
                errors.Add(new FieldError { Field = "email", Message = "Email already exists." });

                return new ValidationResponseDto
                {
                    Success = false,
                    Errors = errors
                };
            }

            // Upload slike
            var profileUrl = await _profilePictureBlobService.UploadFileAsync(dto.ProfileImage);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FullName = dto.FullName,
                Gender = dto.Gender,
                Country = dto.Country,
                City = dto.City,
                Address = dto.Address,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                ProfilePictureUrl = profileUrl
            };

            await _userRepo.SaveUserAsync(user);

            return new ValidationResponseDto
            {
                Success = true,
                Errors = new List<FieldError>()
            };
        }
    }
}