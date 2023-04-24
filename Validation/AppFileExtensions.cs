using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnJwtAuth.Validation
{
    public class AppFileExtensions : ValidationAttribute
    {
        public string[] AllowMimeTypes { get; set; }

        public AppFileExtensions(params string[] allowMimeTypes)
        {
            AllowMimeTypes = allowMimeTypes;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file && value != null)
            {
                if (!AllowMimeTypes.Contains(file.ContentType))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }

    }
}