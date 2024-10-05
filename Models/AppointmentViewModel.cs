using Garage88.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class AppointmentViewModel : Appointment
    {
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Technician")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Technician.")]
        public int MechanicId { get; set; }

        public int EstimateId { get; set; }

        public int ClientId { get; set; }

        public int VehicleId { get; set; }

        public IEnumerable<SelectListItem> Technicians { get; set; }
    }
}
