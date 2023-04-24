using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LearnJwtAuth.Entities
{
    [Table("users")]
    [Index(nameof(AppUser.Username), IsUnique = true)]
    public class AppUser : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public byte[]? PasswordHash { get; set; }

        [Required]
        public byte[]? PasswordSalt { get; set; }

        [MaxLength(255)]
        public string? RefreshToken { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TokenCreated { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TokenExpires { get; set; }

        [Required]
        public int RoleId { get; set; }

        public Role? Role { get; set; }

    }
}