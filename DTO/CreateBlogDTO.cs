using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.Entities;
using LearnJwtAuth.Validation;
using Microsoft.AspNetCore.Mvc;

namespace LearnJwtAuth.DTO
{
    public class CreateBlogDTO
    {
        [Required(ErrorMessage = "Please choose one of categories.")]
        [ForeignKey(nameof(Category.Id))]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }

        public string? Content { get; set; } = "";

        public ICollection<string>? TagsName { get; set; } = new List<string>();

        [FromForm(Name = "Image")]
        [AppFileExtensions(AllowMimeTypes = new string[] { "image/png", "image/jpeg" }, ErrorMessage = "Only jpeg and png are allowed.")]
        [FileSize(5 * 1024 * 1024)]
        public IFormFile? Image { get; set; }
    }
}