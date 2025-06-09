using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Otp { get; set; } = null!;
        [Required, MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
}
