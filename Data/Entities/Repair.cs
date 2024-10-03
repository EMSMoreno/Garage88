using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Garage88.Data.Entities
{
    public class Repair : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
        public static readonly CultureInfo UnitedStates =
        CultureInfo.GetCultureInfo("en-US");

        [Required]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        public string? MechanicId { get; set; }

        public string? ClientId { get; set; }

        [Required]
        [Display(Name = "Discount(%)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal Discount { get; set; }

    }
}
