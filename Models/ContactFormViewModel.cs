using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class ContactFormViewModel
    {
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Phone Number")]
        [Required]
        public string PhoneNumber { get; set; }
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Message")]
        [Required]
        public string Message { get; set; }
    }
}
