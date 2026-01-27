using SimpleLMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Application.DTOs.Users
{
    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
