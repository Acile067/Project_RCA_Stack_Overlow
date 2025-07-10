using StackOverflow.API.Areas.HelpPage;
using StackOverflow.Application.Features.Token.Command;
using StackOverflow.Application.Features.User.Commands.RegisterUser;
using StackOverflow.Application.Features.User.Commands.UpdateUser;
using StackOverflow.Application.Features.User.Quieres.GetUserById;
using StackOverflow.Application.Features.User.Quieres.ProfilePictureQuiery;
using StackOverflow.Infrastructure.Common;
using StackOverflow.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        private readonly UserProfilePictureService _userProfilePictureService;
        private readonly GetUserByIdService _getUserByIdService;
        private readonly UpdateUserService _updateUserService;

        public UserController()
        {
            var userTableContext = new UserTableContext();
            var profilePictureBlobContext = new ProfilePictureBlobContext();
            var repo = new RegisterRepository(userTableContext, profilePictureBlobContext);
            _userService = new RegisterUserService(repo);
            _createTokenService = new CreateTokenService(new LoginRepository(userTableContext));
            _userProfilePictureService = new UserProfilePictureService(new ProfilePictureRepository(profilePictureBlobContext));
            _getUserByIdService = new GetUserByIdService(new UserRepository(userTableContext, profilePictureBlobContext));
            _updateUserService = new UpdateUserService(new UserRepository(userTableContext, profilePictureBlobContext), new ProfilePictureRepository(profilePictureBlobContext));

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
                Gender = await GetFormValue(form, "gender"),
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
        public async Task<IHttpActionResult> LoginAsync([FromBody] CreateTokenDTO createTokenDTO)
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

        [Authorize]
        [HttpGet, Route("user/profileimage")]
        public async Task<IHttpActionResult> GetProfileImage()
        {
            var identity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var fileName = identity?.Claims.FirstOrDefault(c => c.Type == "profilePictureFileName")?.Value;

            if (string.IsNullOrEmpty(fileName))
                return BadRequest("Profile picture filename not found in token.");

            try
            {
                var imageBytes = await _userProfilePictureService.GetProfilePictureAsync(fileName);
                if (imageBytes == null || imageBytes.Length == 0)
                    return NotFound();

                string contentType = GetMimeTypeFromFileName(fileName);

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(imageBytes)
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        private string GetMimeTypeFromFileName(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (extension == ".jpg" || extension == ".jpeg")
                return "image/jpeg";
            else if (extension == ".png")
                return "image/png";
            else if (extension == ".gif")
                return "image/gif";
            else if (extension == ".webp")
                return "image/webp";
            else
                return "application/octet-stream";
        }

        [Authorize]
        [HttpGet, Route("user/{id}")]
        public async Task<IHttpActionResult> GetUserById(string id)
        {
            var identity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var tokenid = identity?.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (tokenid != id)
            {
                return Unauthorized();
            }


            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            try
            {
                var user = await _getUserByIdService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize]
        [HttpPut, Route("user/{id}")]
        public async Task<IHttpActionResult> UpdateUser(string id)
        {
            var identity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var oldProfilePicture = identity?.Claims.FirstOrDefault(c => c.Type == "profilePictureFileName")?.Value;

            if (string.IsNullOrEmpty(oldProfilePicture))
                return BadRequest("Profile picture filename not found in token.");

            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest("Unsupported media type");
            }
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            // Ekstrakcija podataka iz forme
            var form = provider.Contents;
            // Parsiranje form polja
            var updateUserDTO = new UpdateUserDTO
            {
                Id = id,
                Name = await GetFormValue(form, "name"),
                LastName = await GetFormValue(form, "lastName"),
                Gender = await GetFormValue(form, "gender"),
                Country = await GetFormValue(form, "country"),
                City = await GetFormValue(form, "city"),
                Address = await GetFormValue(form, "address"),
                Email = await GetFormValue(form, "email"),
                Password = await GetFormValue(form, "password"),
                OldProfilePictureFileName = oldProfilePicture
            };

            // Dobavljanje slike
            var imageContent = form.FirstOrDefault(c => c.Headers.ContentDisposition.Name.Trim('\"') == "profileImage");
            if (imageContent != null)
            {
                updateUserDTO.ProfilePictureFileName = imageContent.Headers.ContentDisposition.FileName.Trim('\"');
                updateUserDTO.ProfilePictureContent = await imageContent.ReadAsByteArrayAsync();
            }

            try
            {
                var ret = await _updateUserService.UpdateUserAsync(updateUserDTO);
                if (!ret)
                {
                    return BadRequest("User update failed");
                }
                return Ok("User updated successfully");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}