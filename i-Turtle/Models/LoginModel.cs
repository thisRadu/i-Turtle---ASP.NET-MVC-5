using System.ComponentModel.DataAnnotations;

namespace i_Turtle.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Name")]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
