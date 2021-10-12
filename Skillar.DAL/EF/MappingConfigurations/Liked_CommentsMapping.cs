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
    class Liked_CommentsMapping : IEntityTypeConfiguration<Liked_Comments>
    {
        public void Configure(EntityTypeBuilder<Liked_Comments> builder)
        {

            builder.ToTable("LikedComments", "dbo");

            builder.HasKey(x => new { x.Id });

            builder.Property(p => p.Score)
               .IsRequired();

            builder.HasOne(x => x.ApplicationUser)
                .WithMany(x => x.CommentsLiked)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Comment)
                .WithMany(x => x.CommentsLiked)
                .HasForeignKey(x => x.CommentId);
        }
    }
}
