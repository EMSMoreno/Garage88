using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class Vehicle : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Brand")]
        public Brand Brand { get; set; }

        //Model FK
        public int ModelId { get; set; }

        [Required]
        [Display(Name = "Model")]
        public Model Model { get; set; }

        [Display(Name = "Date of Construction")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime DateOfConstruction { get; set; }

        [Required]
        [MaxLength(8, ErrorMessage = "{0} can only have {1} characters.")]
        [MinLength(6, ErrorMessage = "{0} needs to have at least {1} characters.")]
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }

        [MaxLength(17)]
        [Display(Name = "VIN")]
        public string VehicleIdentificationNumber { get; set; }

        public int Horsepower { get; set; }

        public int ClientId { get; set; }

        public Client? Client { get; set; }
    }
}
