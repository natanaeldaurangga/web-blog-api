using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LearnJwtAuth.Entities
{
    [Table("blog_tag")]
    public class BlogTag
    {
        public int BlogId { get; set; }

        public Blog? Blog { get; set; }

        public int TagId { get; set; }

        public Tag? Tag { get; set; }

    }
}