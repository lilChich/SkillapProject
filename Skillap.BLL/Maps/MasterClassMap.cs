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
    public class MasterClassMap : Profile
    {
        public MasterClassMap()
        {
            CreateMap<MasterClasses, MasterClassesDTO>()
                .ForMember(DTO => DTO.Id, opt => opt.MapFrom(DO => DO.Id))
                .ForMember(DTO => DTO.Name, opt => opt.MapFrom(DO => DO.Name))
                .ForMember(DTO => DTO.Description, opt => opt.MapFrom(DO => DO.Description))
                .ForMember(DTO => DTO.Category, opt => opt.MapFrom(DO => DO.Category))
                .ForMember(DTO => DTO.Level, opt => opt.MapFrom(DO => DO.Level))
                .ForMember(DTO => DTO.Relevance, opt => opt.MapFrom(DO => DO.Relevance))
                .ReverseMap();
        }
    }
}
