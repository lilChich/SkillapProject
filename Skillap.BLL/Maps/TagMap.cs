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
    public class TagMap : Profile
    {
        public TagMap()
        {
            CreateMap<Tags, TagsDTO>()
                .ForMember(DTO => DTO.Id, opt => opt.MapFrom(DO => DO.Id))
                .ForMember(DTO => DTO.Name, opt => opt.MapFrom(DO => DO.Name))
                .ReverseMap();
        }
    }
}
