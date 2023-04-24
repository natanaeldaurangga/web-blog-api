using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.Validation;

namespace LearnJwtAuth.DTO
{
    [PasswordMatch]
    public class ChangePasswordDTO
    {
        public string? Password { get; set; }

        public string? RepeatPassword { get; set; }
    }
}