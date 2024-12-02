using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class SpecialityViewModel
    {
        public int RoleId { get; set; }

        public int SpecialityId { get; set; }

        [Required]
        [Display(Name = "speciality")]
        [MaxLength(50, ErrorMessage = "The {0} may only contain {1} characters.")]
        public string Name { get; set; }

    }
}
