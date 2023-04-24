using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LearnJwtAuth.Entities
{
    [Table("categories")]
    [Index(nameof(Category.Name), IsUnique = true)]
    public class Category : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        [JsonIgnore]
        public ICollection<Blog>? Blogs { get; set; }
    }
}