﻿using StackOverflowService.DTOs;
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
        public async Task<IHttpActionResult> GetAllAsync()
        {
            var ret = await _questionService.GetAllAsync();
            return Ok(ret);
        }
        [Authorize]
        [HttpGet]
        [Route("api/questions/{id}")]
        public async Task<IHttpActionResult> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Invalid ID.");

            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
                return NotFound();

            return Ok(question);
        }
        [Authorize]
        [HttpPut]
        [Route("api/questions/edit/{id}")]
        public async Task<IHttpActionResult> Edit(string id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var email = identity.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Invalid ID.");

            var dto = new UpdateQuestionDto()
            {
                Title = HttpContext.Current.Request.Form["title"],
                Description = HttpContext.Current.Request.Form["description"],
                QuestionImage = HttpContext.Current.Request.Files["questionImage"],
            };

            var result = await _questionService.UpdateQuestionAsync(id, dto, email);

            if (result.Success)
                return Ok("Question updated successfully.");
            else
                return Content(HttpStatusCode.BadRequest, result);
        }

        [Authorize]
        [HttpDelete]
        [Route("api/questions/delete/{id}")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var email = identity.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Invalid ID.");

            var success = await _questionService.DeleteQuestionAsync(id, email);
            if (success)
                return Ok("Question deleted successfully.");
            else
                return NotFound();
        }
        [Authorize]
        [HttpGet]
        [Route("api/questions/my-questions")]
        public async Task<IHttpActionResult> GetMyQuestions()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var email = identity.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var questions = await _questionService.GetQuestionsByUserEmailAsync(email);
            return Ok(questions);
        }
        [Authorize]
        [HttpGet]
        [Route("api/questions/search")]
        public async Task<IHttpActionResult> Search([FromUri] string title = "", [FromUri] DateTime? from = null, [FromUri] DateTime? to = null)
        {
            var results = await _questionService.SearchQuestionsAsync(title, from, to);
            return Ok(results);
        }
    }
}
