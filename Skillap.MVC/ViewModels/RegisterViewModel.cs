using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Skillap.MVC.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [PersonalData]
        [StringLength(25, MinimumLength = 2)]
        [Display(Name = "First Name")]
        [RegularExpression("^[a-zA-Zа-яА-Я]{2,30}$|^$",
            ErrorMessage = "Name must contains from 2 to 30 acharacters")]
        public string FirstName { get; set; }

        [Required]
        [PersonalData]
        [StringLength(25, MinimumLength = 2)]
        [Display(Name = "Second Name")]
        [RegularExpression("^[a-zA-Zа-яА-Я]{2,30}$|^$",
            ErrorMessage = "Name must contains from 2 to 30 acharacters")]
        public string SecondName { get; set; }

        [Required]
        public bool Gender { get; set; }

        [Required]
        [StringLength(35, MinimumLength = 2)]
        public string Country { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 10)]
        public string Education { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Day Of Birth")]
        public DateTime? DayOfBirth { get; set; }

        [Display(Name = "Nickname")]
        public string NickName { get; set; }

        [Display(Name = "Image")]
        public IFormFile Image { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 13)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool RememberMe { get; set; }
    }
}
