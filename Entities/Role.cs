using System.Data;
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
    [Table("roles")]
    [Index(nameof(Role.Name), IsUnique = true)]
    // TODO: Lanjut untuk migrations pake base entity
    public class Role : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }

        [JsonIgnore]
        public ICollection<AppUser>? Users { get; set; }
    }
}