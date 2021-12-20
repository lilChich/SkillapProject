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
    public class MasterMap : Profile
    {

        public MasterMap()
        {
            CreateMap<Masters, MastersDTO>()
                .ForMember(DTO => DTO.Id, opt => opt.MapFrom(DO => DO.Id))
                .ForMember(DTO => DTO.MasterClassId, opt => opt.MapFrom(DO => DO.MasterClassId))
                .ForMember(DTO => DTO.ApplicationUserId, opt => opt.MapFrom(DO => DO.ApplicationUserId))
                .ForMember(DTO => DTO.SkillLevel, opt => opt.MapFrom(DO => DO.SkillLevel))
                .ForMember(DTO => DTO.Status, opt => opt.MapFrom(DO => DO.Status))
                .ReverseMap();
        }
    }
}
