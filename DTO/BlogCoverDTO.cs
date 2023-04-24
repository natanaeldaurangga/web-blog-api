using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnJwtAuth.DTO
{
    public class BlogCoverDTO
    {
        public string? SecureId { get; set; }

        public string? Title { get; set; }

        public string? Username { get; set; }

        public string? CreatorName { get; set; }

        public ICollection<string> Tags { get; set; } = new List<string>();

        public string? ImagePath { get; set; }
    }
}