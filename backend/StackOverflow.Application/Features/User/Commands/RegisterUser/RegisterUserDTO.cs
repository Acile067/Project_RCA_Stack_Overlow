using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Application.Features.User.Commands.RegisterUser
{
    public class RegisterUserDTO
    {
        public string Name { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string Country { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string City { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
