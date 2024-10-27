using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }
}
