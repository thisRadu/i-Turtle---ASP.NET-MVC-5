using System;
using System.Collections.Generic;

#nullable disable

namespace i_Turtle.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public bool Active { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? TwoFactorCode { get; set; }   
    }
}
