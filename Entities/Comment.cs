using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LearnJwtAuth.Entities
{
    [Table("comments")]
    public class Comment : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public virtual AppUser? User { get; set; }

        public string? Content { get; set; }

        public int BlogId { get; set; }

        public virtual Blog? Blog { get; set; } // NOTE: Keyword virtual digunakan untuk LazyLoading
    }
}