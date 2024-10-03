using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class Billing : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Estimate Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime EstimateDate { get; set; }

        [Required]
        public Client? Client { get; set; }

        [Required]
        public User? CreatedBy { get; set; }

        [Required]
        public Vehicle? Vehicle { get; set; }

        public bool HasAppointment { get; set; }
    }
}
