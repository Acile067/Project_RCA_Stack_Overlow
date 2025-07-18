using StackOverflowService.DTOs;
using StackOverflowService.Repositories;
using StackOverflowService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace StackOverflowService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private readonly UserService _userService;

        public UserController()
        {
            var userRepo = new UserRepository();
            var blobService = new ProfilePictureBlobService();
            _userService = new UserService(userRepo, blobService);
        }

        [HttpPost]
        [Route("api/users/create")]
        public async Task<IHttpActionResult> Create()
        {
            var dto = new CreateUserDto
            {
                FullName = HttpContext.Current.Request.Form["fullName"],
                Gender = HttpContext.Current.Request.Form["gender"],
                Country = HttpContext.Current.Request.Form["country"],
                City = HttpContext.Current.Request.Form["city"],
                Address = HttpContext.Current.Request.Form["address"],
                Email = HttpContext.Current.Request.Form["email"],
                Password = HttpContext.Current.Request.Form["password"],
                ProfileImage = HttpContext.Current.Request.Files["profileImage"]
            };

            var result = await _userService.CreateUserAsync(dto);

            if (result.Success)
                return Ok("User created successfully.");
            else
                return Content(HttpStatusCode.BadRequest, result);
        }
        [HttpPost]
        [Route("api/users/login")]
        public async Task<IHttpActionResult> Login(LoginDto dto)
        {
            var user = await _userService.GetUserByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Content(HttpStatusCode.BadRequest, new { message = "Invalid credentials." });
            }

            var token = await _userService.GenerateJwtToken(user);

            return Ok(new
            {
                token,
                message = "ok"
            });
        }
        [Authorize]
        [HttpGet]
        [Route("api/users/test/autorize")]
        public IHttpActionResult GetProfile()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var email = identity.FindFirst(ClaimTypes.Email)?.Value;
            var id = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(new { email, id });
        }
    }
}
