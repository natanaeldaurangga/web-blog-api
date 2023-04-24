using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.DTO;
using LearnJwtAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnJwtAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _service;

        private readonly ILogger<TagController> _logger;

        public TagController(ITagService service, ILogger<TagController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddNewTask(string name)
        {
            try
            {
                var result = await _service.AddTag(name);
                return Ok(new TagDTO
                {
                    Name = result.Name,
                    CreatedAt = result.CreatedAt,
                    UpdatedAt = result.UpdatedAt
                });
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, error.Message);
            }
        }

        [HttpPost("InsertBatch"), Authorize]
        public async Task<IActionResult> AddAllNewTask(ICollection<string> names)
        {
            try
            {
                var result = await _service.FindOrCreateTags(names);
                _logger.LogInformation("Length result: {}", result.Count());
                return Ok(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, error.Message);
            }
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetTags([FromQuery] PageQueryDTO dto)
        {
            try
            {
                var result = await _service.GetAllTagsAsync(dto);
                return Ok(result);
            }
            catch (BadHttpRequestException error)
            {
                return BadRequest(error.Message);
            }
            catch (System.Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{name}"), Authorize("ADMIN")]
        public async Task<IActionResult> DeleteTags([FromRoute] string name)
        {
            try
            {
                var result = await _service.DeleteTag(name);
                return Ok(result);
            }
            catch (BadHttpRequestException error)
            {
                return BadRequest(error.Message);
            }
            catch (System.Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}