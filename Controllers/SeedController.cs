using System.Security.Cryptography;
using LearnJwtAuth.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using LearnJwtAuth.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearnJwtAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AddAllOrigins")]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeedController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> SeedDatabase()
        {
            // SEED ROLES
            _context.Roles!.AddRange(
                new Role { Name = "ROLE_ADMIN" },
                new Role { Name = "ROLE_USER" }
            );

            await _context.SaveChangesAsync();

            // SEED USERS
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name!.Equals("ROLE_ADMIN"));
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name!.Equals("ROLE_USER"));
            const string password = "Test1234";
            using var hmac = new HMACSHA256();
            var passwordSalt = hmac.Key;
            var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            var admin = new AppUser
            {
                Name = "Natanael Daurangga",
                Username = "nael_dau",
                RoleId = adminRole!.Id,
                Role = adminRole,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            var user = new AppUser
            {
                Name = "Steven Marulitua",
                Username = "epen_cupen",
                RoleId = userRole!.Id,
                Role = userRole,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _context.Users!.AddRange(
                admin, user
            );

            await _context.SaveChangesAsync();

            string[] usersName = { "Endang", "Budi", "Sanita", "Si", "Ring", "Oda" };
            string[] usernames = { "endang.jambeh", "budi.setiadi", "san.ita", "sisiuk", "rin", "oda_noob" };

            var users = new List<AppUser>();
            for (int i = 0; i < usernames.Length; i++)
            {
                users.Add(new AppUser
                {
                    Name = usersName[i],
                    Username = usernames[i],
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    RoleId = userRole!.Id,
                    Role = userRole
                });
            }

            _context.Users!.AddRange(users);

            await _context.SaveChangesAsync();

            // SEED TAGS
            _context.Tags.AddRange(
                new Tag { Name = "reactjs-beginner" },
                new Tag { Name = "javascript-async-await" },
                new Tag { Name = "spring-boot-for-beginner" },
                new Tag { Name = "asp-net-core-tutorial" },
                new Tag { Name = "how-to-pet" },
                new Tag { Name = "learn-cooking" }
            );

            await _context.SaveChangesAsync();

            // SEED CATEGORY
            _context.Categories.AddRange(
                new Category { Name = "tech-n-tool" },
                new Category { Name = "life-style" },
                new Category { Name = "music" },
                new Category { Name = "documenter" },
                new Category { Name = "comedy-n-politic" }
            );

            await _context.SaveChangesAsync();
            return Ok("Seeding success");
        }
    }
}