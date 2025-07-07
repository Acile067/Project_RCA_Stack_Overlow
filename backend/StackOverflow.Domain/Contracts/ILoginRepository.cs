using StackOverflow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.Contracts
{
    public interface ILoginRepository
    {
        Task<TokenUserDto> LoginAsync(string email, string password);
    }
}
