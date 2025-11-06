using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;

namespace API.DTOs.Account
{
    public class RegisterStaffDto
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required] public string FullName { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Required] public string Role { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}