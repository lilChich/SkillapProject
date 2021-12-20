using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Skillap.BLL.DTO;
using Skillap.MVC.ViewModels;

namespace Skillap.MVC.Maps
{
    public class WebMappings : Profile
    {
        public WebMappings() 
        {
            CreateMap<UserDTO, UserViewModel>();
        }
    }
}
