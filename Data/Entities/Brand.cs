using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.Entities
{
    public class Brand : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The {0} may only contain {1} characters.")]
        public string? Name { get; set; }

        public ICollection<Model> ? Models { get; set; }

        public int NumberOfModels => Models == null ? 0 : Models.Count;
    }
}
