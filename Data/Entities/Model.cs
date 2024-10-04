using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class Model
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Model")]
        [MaxLength(50, ErrorMessage = "The {0} may only contain {1} characters.")]
        public string? Name { get; set; }
    }
}
