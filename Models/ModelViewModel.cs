using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class ModelViewModel
    {
        public int BrandId { get; set; }

        public int ModelId { get; set; }

        [Required]
        [Display(Name = "Model")]
        [MaxLength(50, ErrorMessage = "The {0} may only contain {1} characters.")]
        public string Name { get; set; }
    }
}
