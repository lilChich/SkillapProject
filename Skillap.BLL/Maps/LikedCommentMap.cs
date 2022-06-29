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
    public class LikedCommentMap : Profile
    {
        public LikedCommentMap()
        {
            CreateMap<Liked_Comments, Liked_CommentsDTO>()
                .ForMember(DTO => DTO.Id, opt => opt.MapFrom(DO => DO.Id))
                .ForMember(DTO => DTO.Like, opt => opt.MapFrom(DO => DO.Like))
                .ForMember(DTO => DTO.CommentId, opt => opt.MapFrom(DO => DO.CommentId))
                .ForMember(DTO => DTO.Score, opt => opt.MapFrom(DO => DO.Score))
                .ForMember(DTO => DTO.UserId, opt => opt.MapFrom(DO => DO.UserId))
                .ForMember(DTO => DTO.isCreator, opt => opt.MapFrom(DO => DO.isCreator))
                .ReverseMap();
        }
    }
}
