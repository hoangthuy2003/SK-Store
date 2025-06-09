using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class VerifyEmailDto
    {
        [Required, StringLength(6)]
        public string Otp { get; set; } = null!;
    }
}
