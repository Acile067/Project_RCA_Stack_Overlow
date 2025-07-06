using Microsoft.WindowsAzure.Storage.Table;
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
    public class RegisterRepository : IRegisterRepository
    {
        private readonly UserTableContext _userTableContext;
        private readonly ProfilePictureBlobContext _profilePictureBlobContext;
        public RegisterRepository(UserTableContext userTableContext, ProfilePictureBlobContext profilePictureBlobContext)
        {
            _userTableContext = userTableContext ?? throw new ArgumentNullException(nameof(userTableContext));
            _profilePictureBlobContext = profilePictureBlobContext ?? throw new ArgumentNullException(nameof(profilePictureBlobContext)); ;
        }
        public async Task<bool> IsEmailAvailableAsync(string email)
        {       
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            try
            {
                return !await _userTableContext.IsEmailExistingAsync(email);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return false;
            }
        }

        public async Task<bool> RegisterAsync(User user)
        {
            if (user == null)
            {
                return false;
            }

            try
            {
                var ret1 = await _profilePictureBlobContext.UploadImgAsync("profilepictures", user.ProfilePictureFileName, user.ProfilePictureContent);
                var ret2 = await _userTableContext.InsertOrUpdateEntityAsync(user);
                return (ret1 && ret2);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return false;
            }
        }
    }
}
