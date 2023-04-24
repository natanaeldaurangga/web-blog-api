using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.DTO.Enum;
using Microsoft.AspNetCore.Mvc;

namespace LearnJwtAuth.DTO
{
    public class PageQueryDTO
    {
        public int PageSize { get; set; } = 5;

        public int CurrentPage { get; set; } = 1;

        [FromQuery(Name = "Direction")]
        [EnumDataType(typeof(SortDirection))]
        public SortDirection Direction { get; set; } = SortDirection.ASC;

        public string SortBy { get; set; } = string.Empty;

        public string Keyword { get; set; } = string.Empty;
    }
}