using Skillap.DAL.EF;
using Skillap.DAL.Entities;
using Skillap.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private DataContext db = new DataContext();
        private EFGenericRepository<Comments> commentsRepository;
        private EFGenericRepository<Liked_Comments> likedCommentsRepository;
        private EFGenericRepository<Posts> postsRepository;
        private EFGenericRepository<Liked_Posts> likedPostsRepository;
        private EFGenericRepository<Masters> mastersRepository;
        private EFGenericRepository<MasterClasses> masterClassesRepository;
        private EFGenericRepository<Tags> tagsRepository;
        private EFGenericRepository<Post_Tags> postsTagsRepository;
        private EFGenericRepository<ApplicationUsers> applicationUsers;

        public EFUnitOfWork()
        {
        }

        public EFGenericRepository<ApplicationUsers> ApplicationUsers
        {
            get
            {
                if (this.applicationUsers == null)
                {
                    this.applicationUsers = new EFGenericRepository<ApplicationUsers>(db);
                }
                return applicationUsers;
            }
        }

        public EFGenericRepository<Comments> Comments
        {
            get
            {
                if (this.commentsRepository == null)
                {
                    this.commentsRepository = new EFGenericRepository<Comments>(db);
                }
                return commentsRepository;
            }
        }

        public EFGenericRepository<Liked_Comments> LikedComments
        {
            get
            {
                if (this.likedCommentsRepository == null)
                {
                    this.likedCommentsRepository = new EFGenericRepository<Liked_Comments>(db);
                }
                return likedCommentsRepository;
            }
        }

        public EFGenericRepository<Posts> Posts
        {
            get
            {
                if (this.postsRepository == null)
                {
                    this.postsRepository = new EFGenericRepository<Posts>(db);
                }
                return postsRepository;
            }
        }

        public EFGenericRepository<Liked_Posts> LikedPosts
        {
            get
            {
                if (this.likedPostsRepository == null)
                {
                    this.likedPostsRepository = new EFGenericRepository<Liked_Posts>(db);
                }
                return likedPostsRepository;
            }
        }

        public EFGenericRepository<Masters> Masters
        {
            get
            {
                if (this.mastersRepository == null)
                {
                    this.mastersRepository = new EFGenericRepository<Masters>(db);
                }
                return mastersRepository;
            }
        }

        public EFGenericRepository<MasterClasses> MasterClasses
        {
            get
            {
                if (this.masterClassesRepository == null)
                {
                    this.masterClassesRepository = new EFGenericRepository<MasterClasses>(db);
                }
                return masterClassesRepository;
            }
        }

        public EFGenericRepository<Tags> Tags
        {
            get
            {
                if (this.tagsRepository == null)
                {
                    this.tagsRepository = new EFGenericRepository<Tags>(db);
                }
                return tagsRepository;
            }
        }

        public EFGenericRepository<Post_Tags> PostsTags
        {
            get
            {
                if (this.postsTagsRepository == null)
                {
                    this.postsTagsRepository = new EFGenericRepository<Post_Tags>(db);
                }
                return postsTagsRepository;
            }
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}
