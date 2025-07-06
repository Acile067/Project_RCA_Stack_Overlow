using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Domain.Entities
{
    public class User : TableEntity
    {
        public string Id { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string Country { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string City { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        public string ProfilePictureFileName { get; set; } = String.Empty;
        public byte[] ProfilePictureContent { get; set; } = Array.Empty<byte>();

        public User(string indexNo)
        {
            PartitionKey = "User";
            RowKey = indexNo;
        }
        public User()
        {
        }
    }
}
