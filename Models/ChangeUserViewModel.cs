using Garage88.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class ChangeUserViewModel : User
    {
        [Display(Name = "Tax Identification Number / NIF")]
        public string Nif { get; set; }

        public bool HasPassword { get; set; }

    }
}
