using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.Entities;

namespace LearnJwtAuth.DTO
{
    public class BlogDetailDTO
    {
        [MaxLength(100)]
        public string? SecureId { get; set; }

        [MaxLength(255)]
        public string? WriterName { get; set; }

        [MaxLength(255)]
        public string? Username { get; set; }

        [MaxLength(100)]
        public string? CategoryName { get; set; }

        [MaxLength(255)]
        public string? Title { get; set; }

        [MaxLength(255)]
        public string? ImagePath { get; set; }

        [DataType(DataType.Html)]
        public string? Content { get; set; }

        public ICollection<ReadCommentDTO>? Comments { get; set; } = new List<ReadCommentDTO>();

        public ICollection<string>? Tags { get; set; } = new List<string>();
    }
}