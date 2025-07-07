using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Application.Features.Token.Command
{
    public class CreateTokenDTO
    {
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
    public class CreateTokenResponseDTO
    {
        public string Token { get; set; } = String.Empty;
        public bool isSuccess = false;
    }
}
