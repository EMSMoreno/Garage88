using Garage88.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class ChangeUserViewModel : User
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        [Display(Name = "Tax Identification Number / NIF")]
        [Required(ErrorMessage = "NIF is Required.")]
        public string Nif { get; set; }

        public bool HasPassword { get; set; }

        public bool IsClient { get; set; }

        [Required(ErrorMessage = "Please choose a Profile Picture.")]
        public Guid ProfilePicture { get; set; }
    }

    public static class ProfilePictureMapping
    {
        public static Dictionary<Guid, string> Pictures = new Dictionary<Guid, string>
        {
            { Guid.Parse("12345678-1234-1234-1234-1234567890ab"), "~/dist/img/avatar.png" },
            { Guid.Parse("12345678-1234-1234-1234-1234567890ac"), "~/dist/img/avatar2.png" },
            { Guid.Parse("12345678-1234-1234-1234-1234567890ad"), "~/dist/img/avatar3.png" },
            { Guid.Parse("12345678-1234-1234-1234-1234567890ae"), "~/dist/img/avatar4.png" },
            { Guid.Parse("12345678-1234-1234-1234-1234567890af"), "~/dist/img/avatar5.png" }
        };
    }
}