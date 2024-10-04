using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class RoleViewModel
    {
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The {0} may only contain {1} characters.")]
        public string Name { get; set; }

        public IEnumerable<SelectListItem> Permissions { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        //[StringLength(int.MaxValue ,ErrorMessage ="You must select a permission level.",MinimumLength = 2)]
        [Display(Name = "Permission Level")]
        public string SelectedPermission { get; set; }
    }
}
