﻿using System.ComponentModel.DataAnnotations;

namespace Training.Cms.Models
{
    public class LoginVM
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
