using LearnJwtAuth.DTO;
using LearnJwtAuth.DTO.Enum;

namespace LearnJwtAuth.Services
{
    public interface IBlogService
    {
        // TODO: buat service untuk blogs
        public Task<BlogCoverDTO> CreateBlogAsync(string username, CreateBlogDTO dto);

        public Task<PagedResponseDTO<BlogCoverDTO>> GetAllBlogAsync(PageQueryDTO dto, TrashFilter trashFilter = TrashFilter.WithoutTrashed); // TODO: ini pake pagination

        public Task<BlogDetailDTO> FindBlogAsync(string secureId);

        public Task<BlogCoverDTO> DeleteBlogAsync(string secureId);

        public Task<BlogCoverDTO> RestoreBlogAsync(string secureId);

        public Task<BlogCoverDTO> ForceDeleteBlogAsync(string secureId);

        public Task<BlogDetailDTO> UpdateBlogAsync(string secureId, CreateBlogDTO dto);

        public Task<BlogCoverDTO> AddComment(string secureId, string username, CommentDTO dto);
    }

}