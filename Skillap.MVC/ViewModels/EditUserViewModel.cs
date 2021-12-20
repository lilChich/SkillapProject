using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Skillap.MVC.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string SecondName { get; set; }
        public bool Gender { get; set; }
        [Required]
        public string Country { get; set; }
        public string Education { get; set; }
        public DateTime? DayOfBirth { get; set; }
        [Required]
        public string NickName { get; set; }
        public string ExistingPhotoPath { get; set; }
        public IFormFile Image { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public IList<string> Claims { get; set; }
        public IList<string> Roles { get; set; }

        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }
    }
}
