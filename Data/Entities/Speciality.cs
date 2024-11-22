using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class Speciality
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Speciality")]
        [MaxLength(50, ErrorMessage = "The {0} may only contain {1} characters.")]
        public string Name { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; }
    }
}
