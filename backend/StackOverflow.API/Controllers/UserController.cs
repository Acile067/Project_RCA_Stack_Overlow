using StackOverflow.Application.Features.User.Commands.RegisterUser;
using StackOverflow.Infrastructure.Common;
using StackOverflow.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace StackOverflow.API.Controllers
{
    public class UserController : ApiController
    {

        private readonly RegisterUserService _userService;

        public UserController()
        {
            var context = new UserTableContext(); 
            var repo = new RegisterRepository(context); 
            _userService = new RegisterUserService(repo); 
        }

        [HttpPost, Route("user/register")]
        public async Task<IHttpActionResult> CreateUser([FromBody] RegisterUserDTO userDTO)
        {
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
                // Log the exception (not implemented here)
                return InternalServerError(ex);
            }
        }
    }
}