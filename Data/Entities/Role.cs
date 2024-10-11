using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class Role : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The {0} may only contain {1} characters.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Permissions")]
        public string ? PermissionsName { get; set; }

        public ICollection<Speciality> Specialities { get; set; }

        public int NumberOfSpecialities => Specialities == null ? 0 : Specialities.Count;
    }
}
