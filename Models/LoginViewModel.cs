using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
