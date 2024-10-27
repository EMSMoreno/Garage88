using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class ChangeUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        public string Address { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Profile Picture")]
        public Guid ProfilePicture { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool HasPassword { get; set; }

        public bool IsClient { get; set; }

        public string Nif { get; set; }
    }
}