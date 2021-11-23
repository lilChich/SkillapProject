using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skillap.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.EF
{
    public class DataContext : IdentityDbContext<ApplicationUsers, ApplicationRole, int>
    {
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Liked_Comments> LikedComments { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Liked_Posts> LikedPosts { get; set; }
        public DbSet<Masters> Masters { get; set; }
        public DbSet<MasterClasses> MasterClasses { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Post_Tags> PostsTags { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
           : base(options)
        {
        }

        public DataContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=SkillapDB;Trusted_Connection=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            //modelBuilder.Entity<ApplicationUsers>().Property(x => x.Image).IsRequired(false);
            base.OnModelCreating(modelBuilder);
        }
    }
}
