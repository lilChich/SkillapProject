using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Skillap.BLL.Maps;
using System.Threading.Tasks;

namespace Skillap.MVC.Maps
{
    public class CommonMappings
    {
        public static MapperConfiguration InitializeAutoMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MasterClassMap());
                cfg.AddProfile(new WebMappings());
                cfg.AddProfile(new MasterMap());
                cfg.AddProfile(new TagMap());
                cfg.AddProfile(new PostMap());
                cfg.AddProfile(new LikedPostMap());
                cfg.AddProfile(new LikedCommentMap());
            });

            return config;
        }
    }
}
