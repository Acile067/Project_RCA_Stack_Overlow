using StackOverflow.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Infrastructure.Repository
{
    public class LoginRepository : ILoginRepository
    {
        public Task<bool> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}
