using System.Security.Claims;
using LearnJwtAuth.DTO;
using LearnJwtAuth.DTO.Enum;
using LearnJwtAuth.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnJwtAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // TODO: lanjut bikinn UserController untuk blacklist user, delete user, udpate data user
        private readonly IUserService _service;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService service, ILogger<UserController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet, Authorize("ADMIN")]
        public async Task<IActionResult> GetUsers([FromQuery] PageQueryDTO dto)
        {
            try
            {
                var result = await _service.GetAllUsers(dto);
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
        public async Task<IActionResult> GetUsersWithTrashed([FromQuery] PageQueryDTO dto)
        {
            try
            {
                var result = await _service.GetAllUsers(dto, TrashFilter.WithTrashed);
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

        [HttpGet("only-trashed"), Authorize("ADMIN")]
        public async Task<IActionResult> GetUsersOnlyTrashed([FromQuery] PageQueryDTO dto)
        {
            try
            {
                var result = await _service.GetAllUsers(dto, TrashFilter.OnlyTrashed);
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

        [HttpDelete("{username}"), Authorize("ADMIN")]
        public async Task<IActionResult> DeleteUser([FromRoute] string username)
        {
            try
            {
                _logger.LogInformation("UserController.DeleteUser Line 1");
                var result = await _service.SoftDeleteUser(username);
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

        [HttpPut("restore/{username}"), Authorize("ADMIN")]
        public async Task<IActionResult> RestoreUser([FromRoute] string username)
        {
            try
            {
                var result = await _service.RestoreUser(username);
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

        [HttpDelete("force-delete/{username}"), Authorize("ADMIN")]
        public async Task<IActionResult> ForceDeleteUser([FromRoute] string username)
        {
            try
            {
                var result = await _service.ForceDeleteUser(username);
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

        [HttpPut("update-name"), Authorize]
        public async Task<IActionResult> UpdateName([FromBody] ChangeNameDTO dto)
        {
            try
            {
                _logger.LogInformation("UserController.ChangeName Line {}", 1);
                var user = HttpContext.User;
                var username = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                _logger.LogInformation("Username: {}", username);
                var result = await _service.ChangeName(username!, dto);
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

        [HttpPut("change-password"), Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordDTO dto)
        {
            try
            {
                var user = HttpContext.User;
                var username = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var result = await _service.ChangePassword(username!, dto);
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

    }
}