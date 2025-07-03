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
        private readonly UserTableContext _context;
        public RegisterRepository(UserTableContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<bool> IsEmailAvailableAsync(string email)
        {       
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            try
            {
                return !await _context.IsEmailExistingAsync(email);
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
                return await _context.InsertOrUpdateEntityAsync(user);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return false;
            }
        }
    }
}
