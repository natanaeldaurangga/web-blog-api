using System;
using System.Text.RegularExpressions;
using LearnJwtAuth.Data;
using LearnJwtAuth.DTO;
using LearnJwtAuth.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using LearnJwtAuth.DTO.Enum;

namespace LearnJwtAuth.Services.Impl
{
    public class TagService : ITagService
    {
        private readonly AppDbContext _context;

        private readonly ILogger<TagService> _logger;

        public TagService(AppDbContext context, ILogger<TagService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public string NormalizeName(string name)
        {
            return Regex.Replace(name, @"\s", "-").ToLower();
        }

        public async Task<TagDTO> AddTag(string name)
        {
            try
            {
                var tag = new Tag
                {
                    Name = NormalizeName(name)
                };

                var savedTag = _context.Tags.Add(tag);

                await _context.SaveChangesAsync();

                return new TagDTO
                {
                    Name = tag.Name,
                    CreatedAt = tag.CreatedAt,
                    UpdatedAt = tag.UpdatedAt
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ICollection<TagDTO>> AddAllTags(ICollection<string> names)
        {
            try
            {
                var tags = names.Select(n => new Tag { Name = NormalizeName(n) }).ToArray();

                _context.Tags.AddRange(tags);

                await _context.SaveChangesAsync();

                return tags.Select(t => new TagDTO
                {
                    Name = t.Name,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TagDTO> DeleteTag(string name)
        {
            var tag = _context.Tags.FirstOrDefault(t => t.Name!.Equals(name));
            var blogTags = await _context.BlogTags.Where(bt => bt.TagId == tag!.Id).ToArrayAsync();
            _context.BlogTags.RemoveRange(blogTags);
            _context.Tags.Remove(tag!);
            await _context.SaveChangesAsync();
            return new TagDTO
            {
                Name = tag!.Name,
                CreatedAt = tag.CreatedAt,
                UpdatedAt = tag.UpdatedAt
            };
        }

        public async Task<ICollection<string>> FindTagsAsync(string name)
        {
            var tags = await _context.Tags.Select(t => t.Name).Where(
                t => t!.Contains(name)
            ).ToListAsync();
            return tags!;
        }

        public async Task<Tag> FindExactTagAsync(string name)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name!.Equals(name));
            return tag!;
        }

        public async Task<bool> IsTagUniqueAsync(string name)
        {
            var result = await FindExactTagAsync(name);
            return result == default;
        }

        public Tag FindExactTag(string name)
        {
            try
            {
                var tag = _context.Tags.FirstOrDefault(t => t.Name!.Equals(name));
                return tag!;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public bool IsTagUnique(string name)
        {
            var result = FindExactTag(NormalizeName(name));
            return result == default;
        }

        public async Task<ICollection<Tag>> FindOrCreateTags(ICollection<string> names)
        {
            try
            { // TODO: lanjut untuk delete tag
                var uncreatedTags = names.Where(t => IsTagUnique(t)).ToList();
                if (uncreatedTags.Count > 0)
                {
                    await AddAllTags(uncreatedTags);
                }

                names = names.Select(n => NormalizeName(n)).ToList();

                var tags = await _context.Tags.Where(t => names.Contains(t.Name!)).ToListAsync();
                return tags;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /**
            1. Input
            2. Jika input sudah terdaftar register, jika belum create lalu register
        */
        public async Task<IEnumerable<Tag>> RegisterTags(IEnumerable<string> names, string blogSecureId)
        {
            try
            {
                var uncreatedTags = names.Where(t => _context.Tags.Where(tg => tg.Name!.Equals(t)).ToList().Count > 0).ToList();

                if (uncreatedTags.Count > 0)
                {
                    await AddAllTags(uncreatedTags!);
                }

                var tags = await _context.Tags.Where(t => names.Contains(t.Name)).ToListAsync();

                var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.SecureId!.Equals(blogSecureId));

                tags.AddRange(blog!.BlogTags!.Select(bt => bt.Tag).ToList()!);
                blog!.BlogTags = tags.Select(t => new BlogTag { Tag = t, TagId = t.Id })!.ToList();

                await _context.SaveChangesAsync();

                return tags;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedResponseDTO<TagDTO>> GetAllTagsAsync(PageQueryDTO dto)
        {
            var query = _context.Tags.AsQueryable();

            query = query.Where(t => t.DeletedAt == null);

            if (!string.IsNullOrEmpty(dto.Keyword))
            {
                query = query.Where(t => t.Name!.Contains(dto.Keyword, StringComparison.OrdinalIgnoreCase));
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
            .Select(q => new TagDTO
            {
                Name = q.Name,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt
            }).ToListAsync();
            // TODO: Lanjut untuk UPDATE dan DELETE Tag, sama UPDATE dan DELETE blogs, Nambah comment
            float totalPageDec = (float)totalData / dto.PageSize;
            int totalPage = (int)Math.Ceiling(totalPageDec);

            return new PagedResponseDTO<TagDTO>()
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