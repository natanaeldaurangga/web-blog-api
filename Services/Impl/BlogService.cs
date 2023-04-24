using LearnJwtAuth.Data;
using LearnJwtAuth.DTO;
using LearnJwtAuth.DTO.Enum;
using LearnJwtAuth.Entities;
using LearnJwtAuth.Util;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace LearnJwtAuth.Services.Impl
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _context;

        private readonly IImageUtil _imageUtil;

        private readonly ITagService _tagService;

        private readonly ILogger<BlogService> _logger;

        public BlogService(AppDbContext context, IImageUtil imageUtil, ILogger<BlogService> logger, ITagService tagService)
        {
            _context = context;
            _imageUtil = imageUtil;
            _logger = logger;
            _tagService = tagService;
        }

        public async Task<BlogCoverDTO> CreateBlogAsync(string username, CreateBlogDTO dto)
        {
            try
            {
                var fileName = await _imageUtil.UploadImageAsync(dto.Image!);
                var category = await _context.Categories.FindAsync(dto.CategoryId);
                var user = await _context!.Users!.FirstOrDefaultAsync(u => u.Username.Equals(username));
                var tags = await _tagService.FindOrCreateTags(dto.TagsName!);
                var blog = new Blog()
                {
                    SecureId = Guid.NewGuid().ToString(),
                    Title = dto.Title,
                    CategoryId = dto.CategoryId,
                    Category = category,
                    ImagePath = fileName,
                    Content = dto.Content,
                    UserId = user!.Id,
                    User = user
                };

                blog.BlogTags = tags.Select(t => new BlogTag { TagId = t.Id, Tag = t }).ToList();

                _context.Blogs.Add(blog);

                await _context.SaveChangesAsync();

                return new BlogCoverDTO()
                {
                    SecureId = blog.SecureId,
                    CreatorName = user.Name,
                    Username = user.Username,
                    ImagePath = blog.ImagePath,
                    Title = blog.Title,
                    Tags = blog.BlogTags!.ToList().ConvertAll(bt => bt.Tag!.Name)!
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<BlogCoverDTO> DeleteBlogAsync(string secureId)
        {
            try
            {
                var blog = await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.BlogTags)!
                    .ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.SecureId!.Equals(secureId));

                blog!.DeletedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                var tags = blog.BlogTags!.Select(bt => bt.Tag!.Name).ToList();

                return new BlogCoverDTO()
                {
                    SecureId = blog.SecureId,
                    ImagePath = blog.ImagePath,
                    CreatorName = blog.User?.Name,
                    Username = blog.User?.Username,
                    Title = blog.Title,
                    Tags = tags!
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Task<BlogDetailDTO> UpdateBlogAsync(string secureId, CreateBlogDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<BlogDetailDTO> FindBlogAsync(string secureId)
        {
            try
            {
                var blog = await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Comments)!
                    .ThenInclude(c => c.User)
                .Include(b => b.BlogTags)!
                    .ThenInclude(bt => bt.Tag)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.SecureId!.Equals(secureId));
                var comments = blog!.Comments!.Select(c =>
                    new ReadCommentDTO { Content = c.Content, Username = c.User!.Username }
                ).ToList();

                var tags = blog!.BlogTags!.Select(bt => bt.Tag!.Name).ToList();

                return new BlogDetailDTO
                {
                    SecureId = blog!.SecureId,
                    WriterName = blog.User!.Name,
                    Username = blog.User.Username,
                    CategoryName = blog.Category!.Name,
                    Title = blog.Title,
                    ImagePath = blog.ImagePath,
                    Content = blog.Content,
                    Comments = comments,
                    Tags = tags!
                };
            }
            catch (Exception error)
            {
                _logger.LogInformation("Terlempar bro: {}", error.StackTrace);
                throw;
            }
        }

        public async Task<BlogCoverDTO> RestoreBlogAsync(string secureId)
        {
            try
            {
                var blog = await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.BlogTags)!
                    .ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.SecureId!.Equals(secureId));

                blog!.DeletedAt = null;
                await _context.SaveChangesAsync();

                return new BlogCoverDTO()
                {
                    SecureId = blog.SecureId,
                    ImagePath = blog.ImagePath,
                    CreatorName = blog.User?.Name,
                    Username = blog.User?.Username,
                    Title = blog.Title,
                    Tags = blog.BlogTags!.ToList().ConvertAll(bt => bt.Tag!.Name)!
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BlogCoverDTO> ForceDeleteBlogAsync(string secureId)
        {
            try
            {
                var blog = await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.BlogTags)!
                    .ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.SecureId!.Equals(secureId));

                _context.Blogs.Remove(blog!);
                await _context.SaveChangesAsync();

                await _imageUtil.DeleteImageAsync(blog!.ImagePath!);

                return new BlogCoverDTO()
                {
                    SecureId = blog!.SecureId,
                    ImagePath = blog.ImagePath,
                    CreatorName = blog.User?.Name,
                    Username = blog.User?.Username,
                    Title = blog.Title,
                    Tags = blog.BlogTags!.ToList().ConvertAll(bt => bt.Tag!.Name)!
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedResponseDTO<BlogCoverDTO>> GetAllBlogAsync(PageQueryDTO dto, TrashFilter trashFilter = TrashFilter.WithoutTrashed)
        {
            var query = _context.Blogs.Include(b => b.User).AsQueryable();

            query = trashFilter switch
            {
                TrashFilter.WithoutTrashed => query.Where(b => b.DeletedAt == default),
                TrashFilter.OnlyTrashed => query.Where(b => b.DeletedAt != default),
                _ => query
            };

            if (!string.IsNullOrEmpty(dto.Keyword))
            {
                query = query.Where(b =>
                    b.SecureId!.ToLower().Contains(dto.Keyword.ToLower()!) ||
                    b.Title!.ToLower().Contains(dto.Keyword.ToLower()!)
                );
            }

            int totalData = query.Count();

            var sortBy = dto.SortBy;
            var direction = dto.Direction;

            if (string.IsNullOrEmpty(sortBy) || string.IsNullOrWhiteSpace(sortBy)) sortBy = "Title";
            if (direction == default) direction = SortDirection.ASC;

            var sortExpression = $"{sortBy} {direction}";
            query = query.OrderBy(sortExpression);

            query = query.Skip((dto.CurrentPage - 1) * dto.PageSize).Take(dto.PageSize);

            var result = await query
            .Select(q => new BlogCoverDTO
            {
                CreatorName = q.User!.Name,
                Username = q.User!.Username,
                SecureId = q.SecureId,
                Title = q.Title,
                ImagePath = q.ImagePath,
                Tags = q.BlogTags!.Select(bt => bt.Tag!.Name).ToList()!
            }).ToListAsync();

            float totalPageDec = (float)totalData / dto.PageSize;
            int totalPage = (int)Math.Ceiling(totalPageDec);

            return new PagedResponseDTO<BlogCoverDTO>()
            {
                PageSize = dto.PageSize,
                PageNumber = dto.CurrentPage,
                TotalCount = totalData,
                TotalPage = totalPage,
                Items = result
            };
        }

        public async Task<BlogCoverDTO> AddComment(string secureId, string username, CommentDTO dto)
        {
            var blog = await _context.Blogs
            .Include(b => b.Comments)
            .Include(b => b.BlogTags)!
                .ThenInclude(bt => bt.Tag)
            .FirstOrDefaultAsync(b => b.SecureId!.Equals(secureId));

            var user = await _context.Users!
            .FirstOrDefaultAsync(u => u.Username.Equals(username));

            blog!.Comments!.Add(new Comment
            {
                Content = dto.Content,
                User = user,
                UserId = user!.Id
            });

            await _context.SaveChangesAsync();

            return new BlogCoverDTO
            {
                Title = blog.Title!,
                SecureId = blog.SecureId,
                Username = user.Username,
                CreatorName = user.Name,
                ImagePath = blog.ImagePath,
                Tags = blog.BlogTags!.Select(bt => bt.Tag!.Name).ToList()!
            };
        }
    }
}