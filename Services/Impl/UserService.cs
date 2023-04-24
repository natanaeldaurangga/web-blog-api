using System.Security.Cryptography;
using System.Text;
using LearnJwtAuth.Data;
using LearnJwtAuth.DTO;
using LearnJwtAuth.DTO.Enum;
using LearnJwtAuth.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace LearnJwtAuth.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA256();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private async Task<AppUser> GetUser(string username)
        {
            return (await _context!.Users!
                .Include(u => u.Role)
                .Where(u => u.Role!.Name!.Equals("ROLE_USER")) // Mehtod ini khusus untuk retrive role user saja
                .FirstOrDefaultAsync(u => u.Username.Equals(username)))!;
        }

        public async Task<UserDTO> SoftDeleteUser(string username)
        {
            var user = await GetUser(username);
            _logger.LogInformation("User Name: {}", user.Name);
            user!.DeletedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return new UserDTO
            {
                Username = user.Username,
                Name = user.Name,
                RoleName = user.Role!.Name
            };
        }

        public async Task<UserDTO> RestoreUser(string username)
        {
            var user = await GetUser(username);
            user!.DeletedAt = null;
            await _context.SaveChangesAsync();
            return new UserDTO
            {
                Username = user.Username,
                Name = user.Name,
                RoleName = user.Role!.Name
            };
        }

        public async Task<UserDTO> ForceDeleteUser(string username)
        {
            var user = await GetUser(username);
            _context.Users!.Remove(user!);
            await _context.SaveChangesAsync();
            return new UserDTO
            {
                Username = user!.Username,
                Name = user.Name,
                RoleName = user.Role!.Name
            };
        }

        public async Task<UserDTO> ChangePassword(string username, ChangePasswordDTO dto)
        {
            CreatePasswordHash(dto.Password!, out byte[] passwordHash, out byte[] passwordSalt);
            // TODO: Lanjut untuk test change password
            var user = await _context!.Users!
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username.Equals(username));
            user!.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Username = user!.Username,
                Name = user.Name,
                RoleName = user.Role!.Name
            };
        }

        public async Task<UserDTO> ChangeName(string username, ChangeNameDTO dto)
        {
            var user = await _context!.Users!
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username.Equals(username));
            user!.Name = dto.Name!;

            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Username = user!.Username,
                Name = user.Name,
                RoleName = user.Role!.Name
            };
        }

        public async Task<PagedResponseDTO<UserDTO>> GetAllUsers(PageQueryDTO dto, TrashFilter trashFilter = TrashFilter.WithoutTrashed)
        {
            var query = _context.Users!.Include(b => b.Role).AsQueryable();

            query = query.Where(u => u.Role!.Name!.Equals("ROLE_USER"));

            query = trashFilter switch
            {
                TrashFilter.WithoutTrashed => query.Where(b => b.DeletedAt == default),
                TrashFilter.OnlyTrashed => query.Where(b => b.DeletedAt != default),
                _ => query
            };

            if (!string.IsNullOrEmpty(dto.Keyword))
            {
                query = query.Where(b =>
                    b.Name!.ToLower().Contains(dto.Keyword.ToLower()!) ||
                    b.Username!.ToLower().Contains(dto.Keyword.ToLower()!) ||
                    b.Role!.Name!.ToLower().Contains(dto.Keyword.ToLower()!)
                );
            }

            int totalData = query.Count();

            var sortBy = dto.SortBy;
            var direction = dto.Direction;

            if (string.IsNullOrEmpty(sortBy) || string.IsNullOrWhiteSpace(sortBy)) sortBy = "Name";
            if (direction == default) direction = SortDirection.ASC;

            var sortExpression = $"{sortBy} {direction}";
            query = query.OrderBy(sortExpression);

            query = query.Skip((dto.CurrentPage - 1) * dto.PageSize).Take(dto.PageSize);

            var result = await query
            .Select(q => new UserDTO
            {
                Name = q.Name,
                Username = q.Username,
                RoleName = q.Role!.Name
            }).ToListAsync();

            float totalPageDec = (float)totalData / dto.PageSize;
            int totalPage = (int)Math.Ceiling(totalPageDec);

            return new PagedResponseDTO<UserDTO>()
            {
                PageSize = dto.PageSize,
                PageNumber = dto.CurrentPage,
                TotalCount = totalData,
                TotalPage = totalPage,
                Items = result
            };
        }
    }
}