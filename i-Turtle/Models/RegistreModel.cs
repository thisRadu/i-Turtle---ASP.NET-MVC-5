using System.ComponentModel.DataAnnotations;

namespace i_Turtle.Models
{
    public class RegistreModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
