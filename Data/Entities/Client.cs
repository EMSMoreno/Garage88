using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class Client : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Tax Identification Number / NIF")]
        public string ? Nif { get; set; }

        public string ? Address { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string ? Email { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "{0} must be numeric")]
        [Display(Name = "Phone Number")]
        [MaxLength(20)]
        public string ? PhoneNumber { get; set; }

        public User ? User { get; set; }

        public ICollection<Vehicle> ? Vehicles { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
