using StackOverflow.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Application.Features.User.Quieres.GetUserById
{
    public class GetUserByIdService
    {
        private readonly IUserRepository _userRepository;
        public GetUserByIdService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        public async Task<GetUserByIdDTO> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            }
            var user = await _userRepository.GetUserByIdAsync(userId);
            
            
            return new GetUserByIdDTO
            {
                Name = user.Name,
                LastName = user.LastName,
                Gender = user.Gender,
                Country = user.Country,
                Address = user.Address,
                City = user.City,
                Email = user.Email,
            };
        }
    }
}
