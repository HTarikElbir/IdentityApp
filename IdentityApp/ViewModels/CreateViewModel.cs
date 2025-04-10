﻿using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 20 characters.")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password doesnt match")]
        public string ConfirmPassword { get; set;} = string.Empty;
    }
}
