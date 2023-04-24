using LearnJwtAuth.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LearnJwtAuth.Controllers
{
    [ApiController]
    [Route("api/image")]
    public class ImageController : ControllerBase
    {
        private readonly IImageUtil _imageUtil;

        private readonly ILogger<ImageController> _logger;

        public ImageController(IImageUtil imageUtil, ILogger<ImageController> logger)
        {
            _imageUtil = imageUtil;
            _logger = logger;
        }

        [HttpGet("get-image/{fileName}"), Authorize]
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
    }
}