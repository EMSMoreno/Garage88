using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class AddRepairViewModel
    {
        [Display(Name = "Repair")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a repair.")]
        public int RepairId { get; set; }

        [Range(0.0001, double.MaxValue, ErrorMessage = "The Quantity must be a positive number.")]
        public double Quantity { get; set; }

        public int CustomerId { get; set; }

        public int VehicleId { get; set; }

        public IEnumerable<SelectListItem> Repairs { get; set; }

        public bool IsEdit { get; set; }
    }
}
