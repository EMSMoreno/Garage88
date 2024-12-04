using Garage88.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class MechanicViewModel
    {
        public int MechanicId { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "First Name")]
        [MaxLength(25, ErrorMessage = "the field {0} can only contain {1} characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Last Name")]
        [MaxLength(25, ErrorMessage = "the field {0} can only contain {1} characters.")]
        public string LastName { get; set; }

        public string About { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Address { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "{0} must be numeric")]
        [Display(Name = "Phone Number")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Role")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Role")]
        public int RoleId { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Speciality")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Speciality")]
        public int SpecialityId { get; set; }

        public IEnumerable<SelectListItem> Specialities { get; set; }

        public string Color { get; set; }

        public Guid PhotoId { get; set; }

    }
}