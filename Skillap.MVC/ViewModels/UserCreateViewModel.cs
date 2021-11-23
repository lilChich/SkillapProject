using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Skillap.MVC.ViewModels
{
    public class UserCreateViewModel : RegisterViewModel
    {
        public new IFormFile Image { get; set; }
    }
}
