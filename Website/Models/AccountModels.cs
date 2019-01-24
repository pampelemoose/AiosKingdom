using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class LoginView
    {
        [Required]
        [Display(Name = "Username")]
        public string LogUsername { get; set; }
        [Required]
        [Display(Name = "Password")]
        public string LogPassword { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public bool IsModal { get; set; }
    }

    public class RegistrationView
    {
        [Required(ErrorMessage = "Username required")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        private Guid _activationCode = Guid.NewGuid();
        [Required]
        public Guid ActivationCode { get { return _activationCode; } }

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Error : Confirm password does not match with password")]
        public string ConfirmPassword { get; set; }
    }

    public class PreAlphaRegistrationView
    {
        [Required(ErrorMessage = "Email required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ResetPasswordView
    {
        [Required(ErrorMessage = "Old Password required")]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Password required")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm New Password required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("Password", ErrorMessage = "Error : Confirm password does not match with password")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeUsernameView
    {
        [Required(ErrorMessage = "New username Required")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class NewTicketView
    {
        [Required(ErrorMessage = "Content Required")]
        [Display(Name = "Content")]
        public string Content { get; set; }
    }
}