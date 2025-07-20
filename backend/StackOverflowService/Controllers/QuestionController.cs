using StackOverflowService.DTOs;
using StackOverflowService.Repositories;
using StackOverflowService.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;

namespace StackOverflowService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QuestionController : ApiController
    {
        private readonly QuestionService _questionService;
        public QuestionController()
        {
            var questionRepo = new QuestionRepository();
            var blobService = new QuestionPictureBlobService();
            _questionService = new QuestionService(questionRepo, blobService);
        }
        [Authorize]
        [HttpPost]
        [Route("api/questions/create")]
        public async Task<IHttpActionResult> Create()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var email = identity.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var dto = new CreateQuestionDto()
            {
                Title = HttpContext.Current.Request.Form["title"],
                Description = HttpContext.Current.Request.Form["description"],
                QuestionImage = HttpContext.Current.Request.Files["questionImage"],
                CreatedBy = email
            };

            var result = await _questionService.CreateQuestionAsync(dto);

            if (result.Success)
                return Ok("Question created successfully.");
            else
                return Content(HttpStatusCode.BadRequest, result);
        }
        [Authorize]
        [HttpGet]
        [Route("api/questions/get-all")]
        public async Task<IHttpActionResult> GetAll()
        {
            var ret = await _questionService.GetQuestionsAsync();
            return Ok(ret);
        }
    }
}
