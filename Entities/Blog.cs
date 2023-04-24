using System.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace LearnJwtAuth.Entities
{
    [Table("blogs")]
    [Index(nameof(Blog.SecureId), IsUnique = true)]
    public class Blog : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }

        [MaxLength(100)]
        public string? SecureId { get; set; }

        public int UserId { get; set; }

        public AppUser? User { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        [MaxLength(255)]
        public string? Title { get; set; }

        [MaxLength(255)]
        public string? ImagePath { get; set; } = "default.png";

        [DataType(DataType.Html)]
        public string? Content { get; set; }

        public ICollection<Comment>? Comments { get; set; }

        public ICollection<BlogTag>? BlogTags { get; set; }
    }
}