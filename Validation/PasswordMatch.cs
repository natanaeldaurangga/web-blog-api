using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.DTO;

namespace LearnJwtAuth.Validation
{
    public class PasswordMatch : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is ChangePasswordDTO changePasswordDto && changePasswordDto.Password != changePasswordDto.RepeatPassword)
            {
                return new ValidationResult("Password do not match!");
            }
            return ValidationResult.Success;
        }
    }
}