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
    class PostsMapping : IEntityTypeConfiguration<Posts>
    {
        public void Configure(EntityTypeBuilder<Posts> builder)
        {
            builder.ToTable("Posts", "dbo");

            builder.HasKey(x => new { x.Id });

            builder.Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode();

            builder.Property(p => p.Description)
                .HasMaxLength(150)
                .IsRequired()
                .IsUnicode();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.CreatedTime)
                .IsRequired();
        }
    }
}
