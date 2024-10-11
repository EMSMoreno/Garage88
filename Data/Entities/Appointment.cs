using Garage88.Data.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class Appointment : IEntity
    {
        public int Id { get; set; }

        public string? Observations { get; set; }

        public Mechanic? Mechanic { get; set; }

        //Client FK
        public int ClientId { get; set; }

        public Client? Client { get; set; }

        //Vehicle FK
        public int VehicleId { get; set; }

        public Vehicle? Vehicle { get; set; }

        public Estimate? Estimate { get; set; }

        public User? CreatedBy { get; set; }

        public User? UpdatedBy { get; set; }

        [Display(Name = "Appointment Start Date and Time")]
        [Required(ErrorMessage = "Must insert the {0}")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        [NotOnSundaysValidation(ErrorMessage = "The Shop is closed on Sundays")]
        [DayAfterCurrentTimeValidation(ErrorMessage = "Date/Time must be after the current day/time.")]
        [ScheduleValidation(ErrorMessage = "Appointment must be inside bussiness hours.")]
        public DateTime AppointmentStartDate { get; set; }

        [Display(Name = "Appointment End Date and Time")]
        [Required(ErrorMessage = "Must insert the {0}")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        [NotOnSundaysValidation(ErrorMessage = "The Shop is closed on Sundays")]
        [ScheduleValidation(ErrorMessage = "Appointment must be inside bussiness hours.")]
        public DateTime AppointmentEndDate { get; set; }

        public string? AppointmentServicesDetails { get; set; }

        public bool AsAttended { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AppointmentEndDate < AppointmentStartDate || AppointmentEndDate == AppointmentStartDate)
            {
                yield return new ValidationResult("The appointment end Date and Time must be greater then the initial appointment date and time.");
            }
        }

        public bool IsInformed { get; set; }
    }
}
