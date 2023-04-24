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
    [Table("tags")]
    [Index(nameof(Tag.Name), IsUnique = true)]
    // [Index(nameof(Tag.Id), IsUnique = true)]
    public class Tag : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }

        // TODO: Lanjut untuk crud data tags dengan menggunakan service tags, sama tambahin soft delete ke tiap tiap entity yang mengekstend BaseEntity
        public ICollection<BlogTag>? BlogTags { get; set; }
    }
}