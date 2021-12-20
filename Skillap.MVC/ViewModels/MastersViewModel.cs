using Microsoft.AspNetCore.Mvc.Rendering;
using Skillap.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skillap.MVC.ViewModels
{
    public class MastersViewModel
    {
        public bool Status { get; set; }
        public int SkillLevel { get; set; }
        public int MasterClassId { get; set; }
        public int UserId { get; set; }
        public IEnumerable<Masters> Masters { get; set; }

        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> MasterClasses { get; set; }

        public int SelectedUser { get; set; }
        public int SelectedMasterClass { get; set; }
    }
}
