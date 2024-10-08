﻿using System.ComponentModel.DataAnnotations;

namespace Garage88.Models
{
    public class AddUserPasswordViewModel
    {
        public string UserId { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }
    }
}
