using Microsoft.Ajax.Utilities;
using StackOverflowService.AzureStorage;
using StackOverflowService.DTOs;
using StackOverflowService.Entities;
using StackOverflowService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StackOverflowService.Services
{
    public class QuestionService
    {
        private readonly QuestionRepository _questionRepository;
        private readonly QuestionPictureBlobService _pictureBlobService;

        public QuestionService(QuestionRepository questionRepository, QuestionPictureBlobService pictureBlobService)
        {
            _questionRepository = questionRepository;
            _pictureBlobService = pictureBlobService;
        }

        public async Task<ValidationResponseDto> CreateQuestionAsync(CreateQuestionDto dto)
        {
            var errors = new List<FieldError>();

            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add(new FieldError { Field = "title", Message = "Title is required." });
            if (string.IsNullOrWhiteSpace(dto.Description))
                errors.Add(new FieldError { Field = "description", Message = "Description is required." });
            if (dto.QuestionImage == null)
                errors.Add(new FieldError { Field = "questionImage", Message = "Question image is required." });

            if (errors.Any())
            {
                return new ValidationResponseDto
                {
                    Success = false,
                    Errors = errors
                };
            }

            var profileUrl = await _pictureBlobService.UploadFileAsync(dto.QuestionImage);

            var question = new Question()
            {
                Id = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Description = dto.Description,
                PictureUrl = profileUrl,
                CreatedBy = dto.CreatedBy,
                TopAnswerId = null,
                IsClosed = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _questionRepository.SaveQuestionAsync(question);

            return new ValidationResponseDto
            {
                Success = true,
                Errors = new List<FieldError>()
            };
        }
        public async Task<List<QuestionDto>> GetAllAsync()
        {
            var questions = await _questionRepository.GetAllQuestionsAsync();
            return questions.Select(MapToDto).ToList();
        }
        private QuestionDto MapToDto(QuestionTableEntity entity)
        {
            return new QuestionDto
            {
                Id = entity.RowKey,
                Title = entity.Title,
                Description = entity.Description,
                PictureUrl = entity.PictureUrl,
                CreatedBy = entity.CreatedBy,
                TopAnswerId = entity.TopAnswerId,
                IsClosed = entity.IsClosed,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
        public async Task<QuestionDto> GetQuestionByIdAsync(string id)
        {
            var entity = await _questionRepository.GetQuestionByIdAsync(id);
            if (entity == null) return null;

            return new QuestionDto
            {
                Id = entity.RowKey,
                Title = entity.Title,
                Description = entity.Description,
                PictureUrl = entity.PictureUrl,
                CreatedBy = entity.CreatedBy,
                TopAnswerId = entity.TopAnswerId,
                IsClosed = entity.IsClosed,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}