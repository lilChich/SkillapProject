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
    class CommentsMapping : IEntityTypeConfiguration<Comments>
    {
        public void Configure(EntityTypeBuilder<Comments> builder)
        {
            builder.ToTable("Comments", "dbo");

            builder.HasKey(x => new { x.Id });

            builder.Property(p => p.Content)
                .HasMaxLength(300)
                .IsRequired()
                .IsUnicode();

            builder.Property(p => p.CreatedTime)
                .IsRequired();

            builder.HasOne(x => x.Post)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.PostId);
        }
    }
}
