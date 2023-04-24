using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.DTO;
using LearnJwtAuth.Entities;

namespace LearnJwtAuth.Services
{
    public interface ITagService
    {
        public Task<TagDTO> AddTag(string name);

        public Task<ICollection<TagDTO>> AddAllTags(ICollection<string> names);

        public Task<bool> IsTagUniqueAsync(string name);

        public bool IsTagUnique(string name);

        public Task<TagDTO> DeleteTag(string name);

        public Task<Tag> FindExactTagAsync(string name);

        public Tag FindExactTag(string name);

        public Task<ICollection<string>> FindTagsAsync(string name);

        public Task<ICollection<Tag>> FindOrCreateTags(ICollection<string> names);

        public Task<IEnumerable<Tag>> RegisterTags(IEnumerable<string> names, string blogSecureId);

        public Task<PagedResponseDTO<TagDTO>> GetAllTagsAsync(PageQueryDTO dto);
    }
}