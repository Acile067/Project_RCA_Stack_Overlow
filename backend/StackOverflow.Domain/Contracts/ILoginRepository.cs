using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.Contracts
{
    public interface ILoginRepository
    {
        Task<bool> LoginAsync(string email, string password);
    }
}
