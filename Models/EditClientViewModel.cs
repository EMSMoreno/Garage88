using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class EditClientViewModel : ChangeUserViewModel
    {
        public string UserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Tax Identification Number / NIF")]
        public string Nif { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
