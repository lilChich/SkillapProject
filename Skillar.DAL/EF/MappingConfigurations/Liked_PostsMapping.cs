using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skillap.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.EF.MappingConfigurations
{
    class Liked_PostsMapping : IEntityTypeConfiguration<Liked_Posts>
    {
        public void Configure(EntityTypeBuilder<Liked_Posts> builder)
        {
            builder.ToTable("LikedPosts", "dbo");

            builder.HasKey(x => new { x.Id });

            builder.Property(p => p.Score)
                .IsRequired();

            builder.HasOne(x => x.Posts)
                .WithMany(x => x.PostsLiked)
                .HasForeignKey(x => x.PostId);

            builder.HasOne(x => x.Users)
                .WithMany(x => x.PostsLiked)
                .HasForeignKey(x => x.UserId);
        }
    }
}
