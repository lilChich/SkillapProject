using AutoMapper;
using Skillap.BLL.DTO;
using Skillap.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.BLL.Maps
{
    public class PostMap : Profile
    {
        public PostMap()
        {
            CreateMap<Posts, PostsDTO>()
                .ForMember(DTO => DTO.Id, opt => opt.MapFrom(DO => DO.Id))
                .ForMember(DTO => DTO.Name, opt => opt.MapFrom(DO => DO.Name))
                .ForMember(DTO => DTO.Description, opt => opt.MapFrom(DO => DO.Description))
                .ForMember(DTO => DTO.CreatedTime, opt => opt.MapFrom(DO => DO.CreatedTime))
                .ForMember(DTO => DTO.Status, opt => opt.MapFrom(DO => DO.Status))
                .ReverseMap();
        }
    }
}
