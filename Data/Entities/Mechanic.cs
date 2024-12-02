using System.ComponentModel.DataAnnotations;
using System;

namespace Garage88.Data.Entities
{
    public class Mechanic : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "First Name")]
        [MaxLength(25, ErrorMessage = "the field {0} can only contain {1} characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Last Name")]
        [MaxLength(25, ErrorMessage = "the field {0} can only contain {1} characters.")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public Speciality? Speciality { get; set; }
        public int? SpecialityId { get; set; }

        public Role? Role { get; set; }
        public int? RoleId { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        public string About { get; set; }

        [Display(Name = "Photo")]
        public Guid? PhotoId { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public User? User { get; set; }
        public string? UserId { get; set; }

        public string? Color { get; set; }

        public string ImageFullPath => PhotoId != null && PhotoId != Guid.Empty
            ? $"/dist/img/{PhotoId}.jpg"
            : "/dist/img/user1-128x128.jpg";

        // Helper properties to avoid null exceptions in the view
        public string RoleName => Role?.Name ?? "No Role Assigned"; // Handle null safely
        public string SpecialityName => Speciality?.Name ?? "No Speciality Assigned"; // Handle null safely
    }
}