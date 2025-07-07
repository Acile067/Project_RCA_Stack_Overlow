using BCrypt.Net;
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
    public class LoginRepository : ILoginRepository
    {
        private readonly UserTableContext _userTableContext;
        public LoginRepository(UserTableContext userTableContext)
        {
            _userTableContext = userTableContext ?? throw new ArgumentNullException(nameof(userTableContext));
        }
        public async Task<TokenUserDto> LoginAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            try
            {
                var user = await _userTableContext.GetUserByEmailAsync(email);
                if (user == null)
                    return null;

                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return new TokenUserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        LastName = user.LastName,
                        Email = user.Email,
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return null;
            }
        }
    }
}
