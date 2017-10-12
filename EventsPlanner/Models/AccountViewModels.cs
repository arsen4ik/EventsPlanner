using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventsPlanner.Models
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "This field should not be left blank.")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
            ErrorMessage = "Please enter correct email address.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        public int EventId { get; set; }

    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "This field should not be left blank.")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
            ErrorMessage = "Please enter correct email address.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field should not be left blank.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

    }

    public class ConfirmEmailViewModel
    {
        [Required(ErrorMessage = "This field should not be left blank.")]
        [StringLength(100, ErrorMessage = "The length of the password can not be less than {2} characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmation")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string TokenUser { get; set; }



    }

    public class ProfileViewModel
    {
        [StringLength(100, ErrorMessage = "The length of the password can not be less than {2} characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [StringLength(100, ErrorMessage = "The length of the password can not be less than {2} characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

    }

    public class CreateEventViewModel
    {
        public class FieldEvent
        {
            public string Name { get; set; }
            public string Description { get; set; }

        }
        [Required(ErrorMessage = "This field should not be left blank.")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "The date should not be null.")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "The field should not be null.")]
        [Range(typeof(int), "1", "9999999", 
            ErrorMessage = "Please enter number in range 1-9999999.")]
        public int MaxSubscribedUsers { get; set; }

        public List<FieldEvent> Fields { get; set; }

    }
}