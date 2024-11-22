using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class Estimate : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Estimate Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime EstimateDate { get; set; }

        // Foreign key property for Client
        public int ClientId { get; set; }  // Add this property for the foreign key reference

        [Required]
        public Client Client { get; set; }

        [Required]
        public User CreatedBy { get; set; }

        [Required]
        public Vehicle Vehicle { get; set; }

        public IEnumerable<EstimateDetail>? Services { get; set; }

        [Display(Name = "Fault Description")]
        public string? FaultDescription { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public double ValueWithDiscount => Services?.Sum(s => s.ValueWithDiscount) ?? 0;

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value => Services?.Sum(s => s.Value) ?? 0;

        [Display(Name = "Nº Services")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity => Services?.Sum(s => s.Quantity) ?? 0;

        public bool HasAppointment { get; set; }
    }
}
