using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwatsAppCore.Models
{
    public class UserBindingModel
    {
        [Required]
        [Display(Name ="User name")]
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

    public class MessageBindingModel
    {
        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
