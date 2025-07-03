using StackOverflow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.Contracts
{
    public interface IRegisterRepository
    {
        Task<bool> RegisterAsync(User user);
        Task<bool> IsEmailAvailableAsync(string email);
    }
}
