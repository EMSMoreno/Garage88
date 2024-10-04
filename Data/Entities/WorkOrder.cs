using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class WorkOrder : IEntity
    {
        public int Id { get; set; }

        [Required]
        public Appointment Appointment { get; set; }

        [Required]
        [Display(Name = "Arrival Date/Hour")]
        public DateTime OrderDateStart { get; set; }

        [Display(Name = "Finish Date/Hour")]
        public DateTime OrderDateEnd { get; set; }

        public User CreatedBy { get; set; }

        public User UpdatedBy { get; set; }

        public User ServiceDoneBy { get; set; }

        public bool IsFinished { get; set; }

        public bool awaitsReceipt { get; set; }

        public string Observations { get; set; }

        public string Status { get; set; }
    }
}
