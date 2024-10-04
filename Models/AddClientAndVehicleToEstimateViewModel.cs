using Garage88.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class AddClientAndVehicleToEstimateViewModel
    {
        public IEnumerable<SelectListItem> Client { get; set; }

        public List<Client> Clients { get; set; }      

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Client")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Client")]
        public int ClientId { get; set; }

        public IEnumerable<SelectListItem> Vehicles { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Vehicle")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Vehicle")]
        public int VehicleId { get; set; }
    }
}
