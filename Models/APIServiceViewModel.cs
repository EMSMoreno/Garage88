namespace Garage88.Models
{
    public class APIServiceViewModel
    {
        public int Id { get; set; }

        public string PlateNumber { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Vin { get; set; }

        public DateTime ServiceDateStart { get; set; }

        public DateTime ServiceDateEnd { get; set; }

        public bool IsFinished { get; set; }

        public string Observations { get; set; }

        public string Status { get; set; }

        public string ServiceDoneBy { get; set; }
    }
}
