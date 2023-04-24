using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LearnJwtAuth.Entities
{
    public abstract class BaseEntity
    {
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [DefaultValue(null)]
        public DateTime? DeletedAt { get; set; }
    }
}