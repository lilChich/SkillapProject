using Skillap.DAL.Entities;
using Skillap.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable

    {
        EFGenericRepository<Comments> Comments { get; }
        EFGenericRepository<Liked_Comments> LikedComments { get; }
        EFGenericRepository<Posts> Posts { get; }
        EFGenericRepository<Liked_Posts> LikedPosts { get; }
        EFGenericRepository<Masters> Masters { get; }
        EFGenericRepository<MasterClasses> MasterClasses { get; }
        EFGenericRepository<Tags> Tags { get; }
        EFGenericRepository<Post_Tags> PostsTags { get; }
        void Save();
    }
}
