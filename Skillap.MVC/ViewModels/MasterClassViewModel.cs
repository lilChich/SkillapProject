using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Skillap.MVC.ViewModels
{
    public class MasterClassViewModel
    {
        [Required]
        [StringLength(30, MinimumLength = 5)]
        [Display(Name = "Name")]
        [RegularExpression("^[a-zA-Zа-яА-Я]{5,30}$|^$",
            ErrorMessage = "Name must contains from 5 to 30 characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 10)]
        [Display(Name = "Description")]
        [RegularExpression("^(.|s)*[a-zA-Z]+(.|s)*$",
            ErrorMessage = "Description must contains from 10 to 150 characters")]
        public string Description { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        [Display(Name = "Category")]
        public string Category { get; set; }

        public int Relevance { get; set; }

        public int Level { get; set; }

        public List<SelectListItem> Users { get; set; }

        public string SelectedUser { get; set; }
    }
}
