using System.ComponentModel.DataAnnotations;

namespace LearnJwtAuth.Validation
{
    public class FileSize : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public FileSize(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file && value != null)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult($"The file size should not exceed {_maxFileSize} bytes.");
                }
            }

            return ValidationResult.Success;
        }
    }
}