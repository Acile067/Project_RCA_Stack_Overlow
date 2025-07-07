using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System.Text;
using System.Web.Http;
using System.Web.Http.Owin;

[assembly: OwinStartup(typeof(StackOverflow.API.Startup))]

namespace StackOverflow.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var issuer = "http://localhost:5050/";
            var audience = "https://external.com/security";
            var secret = Encoding.UTF8.GetBytes("your_super_secret_key_1234567890!");

            // JWT konfiguracija
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(secret)
                }
            });

            // Postojeća WebApi konfiguracija
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}
