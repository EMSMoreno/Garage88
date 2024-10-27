using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class AnnouncementViewModel
    {
        public IEnumerable<SelectListItem> To { get; set; }

        [Required(ErrorMessage = "You must select a To Destination.")]
        public int ToId { get; set; }

        [Required(ErrorMessage = "Must insert the Subject.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Must insert the Message.")]
        public string Message { get; set; }

        public IFormFile Attachment { get; set; }

    }
}
