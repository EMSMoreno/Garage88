using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Garage88.Data.Entities
{
    public class Mechanic : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "First Name")]
        [MaxLength(25, ErrorMessage = "the field {0} can only contain {1} characters.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Last Name")]
        [MaxLength(25, ErrorMessage = "the field {0} can only contain {1} characters.")]
        public string? LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        public string? Specialty { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        public string? Role { get; set; }

        public string? About { get; set; }

        [Display(Name = "Photo")]
        public Guid PhotoId { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public User User { get; set; }

        public string? UserId { get; set; }

        public int ClientId { get; set; }

        public int VehicleId { get; set; }
    }
}
