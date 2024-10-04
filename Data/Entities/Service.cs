using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Garage88.Data.Entities
{
    public class Service : IEntity
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

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Discount(%)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal Discount { get; set; }

        [Display(Name = "Price with Discount")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal PriceWithDiscount => Price * (1 - Discount / 100);
    }
}
