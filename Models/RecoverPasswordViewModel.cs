using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
