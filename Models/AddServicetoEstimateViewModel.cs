using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class AddServiceToEstimateViewModel
    {
        [Display(Name = "Service")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a service.")]
        public int ServiceId { get; set; }

        [Range(0.0001, double.MaxValue, ErrorMessage = "The Quantity must be a positive number.")]
        public double Quantity { get; set; }

        public int ClientId { get; set; }

        public int VehicleId { get; set; }

        public IEnumerable<SelectListItem> Services { get; set; }

        public bool IsEdit { get; set; }

        public int EstimateId { get; set; }
    }
}
