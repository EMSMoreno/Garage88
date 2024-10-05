using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class CreateClientViewModel : RegisterViewModel
    {
        [Display(Name = "Tax Identification Number / NIF")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "{0} must be numeric")]
        public string Nif { get; set; }
    }
}
