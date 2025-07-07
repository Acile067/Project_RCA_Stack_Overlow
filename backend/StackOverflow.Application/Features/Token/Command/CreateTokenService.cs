using StackOverflow.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;


namespace StackOverflow.Application.Features.Token.Command
{
    public class CreateTokenService
    {
        private readonly ILoginRepository _loginRepository;
        private const string SecretKey = "your_super_secret_key_1234567890!";
        public CreateTokenService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository ?? throw new ArgumentNullException(nameof(loginRepository));
        }
        public async Task<CreateTokenResponseDTO> CreateTokenAsync(CreateTokenDTO createTokenDTO)
        {
            if (createTokenDTO == null)
                throw new ArgumentNullException(nameof(createTokenDTO));

            var retUser = await _loginRepository.LoginAsync(createTokenDTO.Email, createTokenDTO.Password);
            
            if(retUser == null)
            {
                return new CreateTokenResponseDTO
                {
                    Token = null,
                    isSuccess = false,
                };
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id", retUser.Id),
                new Claim("name", retUser.Name),
                new Claim("lastName", retUser.LastName),
                new Claim("email", retUser.Email)
            };

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5050/",
                audience: "https://external.com/security",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new CreateTokenResponseDTO
            {
                Token = tokenString,
                isSuccess = true
            };
        }
    }
}
