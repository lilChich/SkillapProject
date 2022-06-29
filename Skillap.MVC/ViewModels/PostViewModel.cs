using Skillap.BLL.DTO;
using Skillap.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Skillap.MVC.ViewModels
{
    public class PostViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "Name")]
        //[RegularExpression("^[a-zA-Zа-яА-Я]{5,50}$|^$",
            //ErrorMessage = "Name must contains from 5 to 50 characters")]
        public string Name { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 10)]
        [Display(Name = "Description")]
        [RegularExpression("^(.|s)*[a-zA-Z]+(.|s)*$",
            ErrorMessage = "Description must contains from 10 to 150 characters")]
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Status { get; set; }

        //[RegularExpression(@"\B(\#[a-zA-Z0-9]+\b)(?!;)")]
        public string Tags { get; set; }


        public List<Tags> allTags { get; set; }
        public List<MainCommentViewModel> MainComments { get; set; }
    }
}
