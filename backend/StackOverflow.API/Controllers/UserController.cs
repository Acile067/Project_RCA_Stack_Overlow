using StackOverflow.Application.Features.Token.Command;
using StackOverflow.Application.Features.User.Commands.RegisterUser;
using StackOverflow.Infrastructure.Common;
using StackOverflow.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace StackOverflow.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {

        private readonly RegisterUserService _userService;
        private readonly CreateTokenService _createTokenService;

        public UserController()
        {
            var userTableContext = new UserTableContext();
            var profilePictureBlobContext = new ProfilePictureBlobContext();
            var repo = new RegisterRepository(userTableContext, profilePictureBlobContext);
            _userService = new RegisterUserService(repo);
            _createTokenService = new CreateTokenService(new LoginRepository(userTableContext));
        }

        [HttpPost, Route("user/register")]
        public async Task<IHttpActionResult> CreateUser()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest("Unsupported media type");
            }

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            // Ekstrakcija podataka iz forme
            var form = provider.Contents;

            // Parsiranje form polja
            var userDTO = new RegisterUserDTO
            {
                Name = await GetFormValue(form, "name"),
                LastName = await GetFormValue(form, "lastName"),
                Country = await GetFormValue(form, "country"),
                City = await GetFormValue(form, "city"),
                Address = await GetFormValue(form, "address"),
                Email = await GetFormValue(form, "email"),
                Password = await GetFormValue(form, "password"),
            };

            // Dobavljanje slike
            var imageContent = form.FirstOrDefault(c => c.Headers.ContentDisposition.Name.Trim('\"') == "profileImage");

            if (imageContent != null)
            {
                userDTO.ProfilePictureFileName = imageContent.Headers.ContentDisposition.FileName.Trim('\"');
                userDTO.ProfilePictureContent = await imageContent.ReadAsByteArrayAsync();
            }

            try
            {
                var ret = await _userService.RegisterUserAsync(userDTO);

                if (!ret)
                {
                    return BadRequest("User registration failed");
                }

                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        private async Task<string> GetFormValue(IEnumerable<HttpContent> form, string key)
        {
            var content = form.FirstOrDefault(c => c.Headers.ContentDisposition.Name.Trim('\"') == key);
            return content != null ? await content.ReadAsStringAsync() : string.Empty;
        }

        [HttpPost, Route("user/login")]
        public async Task<IHttpActionResult> LoginAsync ([FromBody] CreateTokenDTO createTokenDTO)
        {
            
            if (createTokenDTO == null)
            {
                return BadRequest("Invalid login data");
            }
            
            try
            {
                var tokenResponse = await _createTokenService.CreateTokenAsync(createTokenDTO);
                
                if (tokenResponse == null || !tokenResponse.isSuccess)
                {
                    return Unauthorized();
                }

                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}