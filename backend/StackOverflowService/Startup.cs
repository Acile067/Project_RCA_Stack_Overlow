﻿using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System;
using System.Text;

[assembly: OwinStartup(typeof(StackOverflowService.Startup))]
namespace StackOverflowService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var issuer = "http://localhost:5050/";
            var audience = "http://localhost:3000/";
            var secret = Encoding.UTF8.GetBytes("070342f4-b749-4e1c-ab79-e73cfc2348a2");

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secret),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }
            });
        }
    }
}
