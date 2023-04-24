using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.DTO;
using Microsoft.AspNetCore.Mvc;
using LearnJwtAuth.Services;
using Microsoft.AspNetCore.Authorization;
using LearnJwtAuth.DTO.Enum;

namespace LearnJwtAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _service;

        private readonly ILogger<BlogController> _logger;

        public BlogController(ILogger<BlogController> logger, IBlogService service)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost, Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddBlog([FromForm] CreateBlogDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = HttpContext.User;
                var username = user.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value;
                return Ok(await _service.CreateBlogAsync(username, dto));
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // TODO: Lanjut untuk bikin update data blog
        [HttpDelete("{secureId}"), Authorize("ADMIN")]
        public async Task<IActionResult> DeleteBlog([FromRoute] string secureId)
        {
            try
            {
                BlogCoverDTO result = await _service.DeleteBlogAsync(secureId);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (BadHttpRequestException err)
            {
                return BadRequest(err.Message);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("force-delete/{secureId}")]
        public async Task<IActionResult> ForceDeleteBlog([FromRoute] string secureId)
        {
            try
            {
                var result = await _service.ForceDeleteBlogAsync(secureId);
                return Ok(result);
            }
            catch (BadHttpRequestException err)
            {
                return BadRequest(err.Message);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{secureId}"), Authorize]
        public async Task<IActionResult> FindBlog([FromRoute] string secureId)
        {
            try
            {
                var result = await _service.FindBlogAsync(secureId);
                return Ok(result);
            }
            catch (BadHttpRequestException error)
            {
                return BadRequest(error.Message);
            }
            catch (NullReferenceException error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error Null Reference");
            }
            catch (System.Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetBlogs([FromQuery] PageQueryDTO dto)
        {
            try
            {
                var result = await _service.GetAllBlogAsync(dto);
                return Ok(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("with-trashed"), Authorize("ADMIN")]
        public async Task<IActionResult> GetBlogsWithTrashed([FromQuery] PageQueryDTO dto)
        {
            try
            {
                var result = await _service.GetAllBlogAsync(dto, TrashFilter.WithTrashed);
                return Ok(result);
            }
            catch (BadHttpRequestException error)
            {
                return BadRequest(error.Message);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("only-trashed"), Authorize("ADMIN")]
        public async Task<IActionResult> GetBlogsOnlyTrashed([FromQuery] PageQueryDTO dto)
        {
            try
            {
                var result = await _service.GetAllBlogAsync(dto, TrashFilter.OnlyTrashed);
                return Ok(result);
            }
            catch (BadHttpRequestException error)
            {
                return BadRequest(error.Message);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("restore/{secureId}"), Authorize("ADMIN")]
        public async Task<IActionResult> RestoreBlog([FromRoute] string secureId)
        {
            try
            {
                var result = await _service.RestoreBlogAsync(secureId);
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

        [HttpPut("comment/{secureId}"), Authorize]
        public async Task<IActionResult> CommentOnBlog([FromRoute] string secureId, CommentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = HttpContext.User;
                var username = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var result = await _service.AddComment(secureId, username!, dto);
                return Ok(result);
            }
            catch (BadHttpRequestException error)
            {
                return BadRequest(error.Message);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }

}