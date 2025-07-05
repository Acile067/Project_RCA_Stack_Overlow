using StackOverflow.Application.Features.User.Commands.RegisterUser;
using StackOverflow.Infrastructure.Common;
using StackOverflow.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
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
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return InternalServerError(ex);
            }
        }
    }
}