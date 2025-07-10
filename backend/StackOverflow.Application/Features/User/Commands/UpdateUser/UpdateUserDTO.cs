using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string Gender { get; set; } = String.Empty;
        public string Country { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string City { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string ProfilePictureFileName { get; set; } = String.Empty;
        public string OldProfilePictureFileName { get; set; } = String.Empty;
        public string ProfilePictureContentType { get; set; } = "image/png";
        public byte[] ProfilePictureContent { get; set; } = Array.Empty<byte>();
    }
}
