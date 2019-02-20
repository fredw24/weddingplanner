using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
        public class LoginUsers{
            [Required]
            [Display(Name = "Email Address:")]
            public string Email { get; set; }
            [Required]
            [Display(Name = "Password:")]
            public string Password { get; set; }

    }
}