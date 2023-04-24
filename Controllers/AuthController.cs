using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.Data;
using LearnJwtAuth.DTO;
using LearnJwtAuth.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace LearnJwtAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AddAllOrigins")] // AddAllOrigins ada di file Program.cs
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext? _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA256();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private async Task<int> SetRefreshToken(RefreshToken newRefreshToken, AppUser user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
            return await _context!.SaveChangesAsync();
        }

        private string GenerateJwtToken(AppUser user)
        {
            _logger.LogInformation("AuthController.GenerateToken: Line 1");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt:Key").Value);
            // TODO: kenapa security token descriptornya null
            var tokenDescriptor = new SecurityTokenDescriptor();
            tokenDescriptor.Audience = _configuration.GetSection("Jwt:Audience").Value;
            tokenDescriptor.Issuer = _configuration.GetSection("Jwt:Issuer").Value;
            // mengambil claim lalu menambahkannya ke tokenDescriptor
            var usernameClaim = new Claim(ClaimTypes.Name, user.Username);
            var roleClaim = new Claim(ClaimTypes.Role, user.Role!.Name!);
            tokenDescriptor.Subject = new ClaimsIdentity(new Claim[]{
                usernameClaim, roleClaim
            });
            tokenDescriptor.Expires = DateTime.Now.AddHours(5);
            tokenDescriptor.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA256(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDTO dto)
        {
            CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var role = await _context!.Roles!.FirstOrDefaultAsync(r => r.Name!.Equals(dto.Role));

            var user = new AppUser()
            {
                Name = dto.Name,
                Username = dto.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleId = role!.Id,
                Role = role
            };

            _context?.Users?.Add(user);

            await _context!.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _context!.Users!
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username.Equals(dto.Username) && u.DeletedAt == null);
            _logger.LogInformation("User Role: {Value}", user!.Role!.Name);
            if (user == null)
            {
                return BadRequest("Username or password is wrong");
            }

            if (!VerifyPasswordHash(dto.Password, user!.PasswordHash!, user!.PasswordSalt!))
            {
                return BadRequest("Username or password is wrong.");
            }

            string token = GenerateJwtToken(user);

            var refreshToken = GenerateRefreshToken();
            _ = SetRefreshToken(refreshToken, user);

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string username)
        {
            var user = await _context!.Users!
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username.Equals(username));
            _logger.LogInformation("Ini dari refresh token");
            var refreshToken = Request.Cookies["refreshToken"];

            if (!user!.RefreshToken!.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }

            if (user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token Expired.");
            }

            string token = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            _ = SetRefreshToken(newRefreshToken, user);
            return Ok(token);
        }

        [HttpGet("getMe"), Authorize]
        public async Task<IActionResult> GetMe()
        {
            var user = HttpContext.User;
            var username = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            _logger.LogInformation("AuthController.GetMe");
            var me = await _context!.Users!
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username.Equals(username));
            if (me == null)
            {
                return BadRequest("User not found.");
            }
            return Ok(me);
        }
    }
}