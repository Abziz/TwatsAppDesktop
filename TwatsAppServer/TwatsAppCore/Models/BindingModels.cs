using System;
using System.ComponentModel.DataAnnotations;

namespace TwatsAppCore.Models.Binding
{
    public class UserRegistrationBindingModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
    }

    public class SendMessageBindingModel
    {
        [Required]
        public int From { get; set; }

        [Required]
        public int To { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTimeOffset DispatchedAt { get; set; }
    }

}
