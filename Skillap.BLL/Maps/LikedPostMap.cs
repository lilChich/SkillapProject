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
    public class LikedPostMap : Profile
    {
        public LikedPostMap()
        {
            CreateMap<Liked_Posts, Liked_PostsDTO>()
                .ForMember(DTO => DTO.Id, opt => opt.MapFrom(DO => DO.Id))
                .ForMember(DTO => DTO.Like, opt => opt.MapFrom(DO => DO.Like))
                .ForMember(DTO => DTO.PostId, opt => opt.MapFrom(DO => DO.PostId))
                .ForMember(DTO => DTO.Score, opt => opt.MapFrom(DO => DO.Score))
                .ForMember(DTO => DTO.UserId, opt => opt.MapFrom(DO => DO.UserId))
                .ReverseMap();
        }
    }
}
