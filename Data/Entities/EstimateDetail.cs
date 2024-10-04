using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class EstimateDetail : IEntity
    {
        public int Id { get; set; }

        [ForeignKey("Estimate")]
        public int EstimateId { get; set; }

        public Service Service { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }

        public int ClientId { get; set; }

        public int VehicleId { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value => Price * (decimal)Quantity;
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceWithDiscount => Service == null ? Price : Service.PriceWithDiscount;

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public double ValueWithDiscount => (double)PriceWithDiscount * Quantity;
    }
}
