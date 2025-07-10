using StackOverflow.Domain.Contracts;
using StackOverflow.Domain.Entities;
using StackOverflow.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserTableContext _userTableContext;
        private readonly ProfilePictureBlobContext _profilePictureBlobContext;
        public UserRepository(UserTableContext userTableContext, ProfilePictureBlobContext profilePictureBlobContext)
        {
            _userTableContext = userTableContext ?? throw new ArgumentNullException(nameof(userTableContext));
            _profilePictureBlobContext = profilePictureBlobContext;
        }
        public async Task<User> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }
            User user = await _userTableContext.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            if (string.IsNullOrEmpty(user.Id))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(user.Id));
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentException("User email cannot be null or empty.", nameof(user.Email));
            }

            var existingUserWithEmail = await _userTableContext.GetUserByEmailAsync(user.Email);
            if (existingUserWithEmail != null && existingUserWithEmail.Id != user.Id)
            {
                throw new InvalidOperationException($"Email {user.Email} is already in use.");
            }

            bool isUpdated = await _userTableContext.InsertOrUpdateEntityAsync(user);

            if (!isUpdated)
            {
                throw new Exception("Failed to update user in the database.");
            }
            return isUpdated;
        }
    }
}
