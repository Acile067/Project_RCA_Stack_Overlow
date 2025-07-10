using StackOverflow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserAsync(User user);
    }
}
