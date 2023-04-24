using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.DTO;
using LearnJwtAuth.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnJwtAuth.Controllers
{
    [ApiController]
    [Route("api/test-api")]
    public class TestApiController : ControllerBase
    {
        private readonly ILogger<TestApiController> _logger;

        private readonly IWebHostEnvironment _environment;

        private readonly IImageUtil _imageUtil;

        public TestApiController(ILogger<TestApiController> logger, IWebHostEnvironment environment, IImageUtil imageUtil)
        {
            _logger = logger;
            _environment = environment;
            _imageUtil = imageUtil;
        }

        [HttpGet("admin"), Authorize(Policy = "ADMIN")]
        public async Task<IActionResult> GetAdmin()
        {
            return Ok("This is Admin");
        }

        [HttpGet("user"), Authorize(Policy = "USER")]
        public async Task<IActionResult> GetUser()
        {
            return Ok("This is user");
        }

        [HttpGet("env_path")]
        public async Task<IActionResult> GetPath()
        {
            return Ok(Path.Combine(_environment.ContentRootPath, "Files\\file.jpg"));
        }

        [HttpPost("test-upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageTestDTO dto)
        {
            try
            {
                var fileName = await _imageUtil.UploadImageAsync(dto.Image);
                return Ok(fileName);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                _logger.LogError(error.StackTrace);
                _logger.LogError(error.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("get-image/{fileName}")]
        public async Task<IActionResult> GetImage([FromRoute] string fileName)
        {
            try
            {
                var imageData = await _imageUtil.GetImageAsync(fileName);
                string fileExtension = Path.GetExtension(fileName).ToLower();
                string contentType = fileExtension switch
                {
                    ".jpeg" => "image/jpeg",
                    ".jpg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

                return File(imageData, contentType);
            }
            catch (FileNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                _logger.LogError(e.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // TODO: Upload, delete, read image sudah berhasil, tinggal implementasi ke blog crud
        [HttpDelete("delete-image/{fileName}")]
        public async Task<IActionResult> DeleteImage([FromRoute] string fileName)
        {
            try
            {
                await _imageUtil.DeleteImageAsync(fileName);
                return Ok($"{fileName} is deleted.");
            }
            catch (FileNotFoundException)
            {
                return NotFound("File Not Found");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                _logger.LogError(e.Source);
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}