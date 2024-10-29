using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }


        [MaxLength(100, ErrorMessage = "The field {0} cannot have more then {1} characters.")]
        public string? Address { get; set; }

        //public string Nif { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Profile Picture")]
        public Guid ProfilePicture { get; set; }
    }
}
