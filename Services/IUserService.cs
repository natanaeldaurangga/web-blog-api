using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.DTO;
using LearnJwtAuth.DTO.Enum;

namespace LearnJwtAuth.Services.Impl
{
    public interface IUserService
    {
        // TODO: mungkin bisa nambahin email verification, sekalian belajar send email lewat MailKit
        public Task<UserDTO> SoftDeleteUser(string username);

        public Task<UserDTO> RestoreUser(string username);

        public Task<UserDTO> ForceDeleteUser(string username);

        public Task<UserDTO> ChangePassword(string username, ChangePasswordDTO dto);

        public Task<UserDTO> ChangeName(string username, ChangeNameDTO dto);

        public Task<PagedResponseDTO<UserDTO>> GetAllUsers(PageQueryDTO dto, TrashFilter trashFilter = TrashFilter.WithoutTrashed);

    }
}