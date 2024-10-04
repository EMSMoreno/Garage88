using Garage88.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class VehicleViewModel
    {
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Brand")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a brand")]
        public int BrandId { get; set; }

        public IEnumerable<SelectListItem> Brands { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Model")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a brand")]
        public int ModelId { get; set; }

        public IEnumerable<SelectListItem> Models { get; set; }


        [Required(ErrorMessage = "Must insert the {0}.")]
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

        public int Horsepower { get; set; } = 0; // Default value = 0 case user leaves it empty

        public int ClientId { get; set; }

        public Client Client { get; set; }

        public bool IsEstimate { get; set; }
    }
}
