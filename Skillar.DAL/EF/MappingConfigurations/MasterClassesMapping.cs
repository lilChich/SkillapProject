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
    class MasterClassesMapping : IEntityTypeConfiguration<MasterClasses>
    {
        public void Configure(EntityTypeBuilder<MasterClasses> builder)
        {

            builder.ToTable("MasterClasses", "dbo");

            builder.HasKey(x => new { x.Id });

            builder.Property(p => p.Name)
               .HasMaxLength(50)
               .IsRequired()
               .IsUnicode();

            builder.Property(p => p.Description)
              .HasMaxLength(300)
              .IsRequired()
              .IsUnicode();

            builder.Property(p => p.Category)
              .HasMaxLength(30)
              .IsRequired()
              .IsUnicode();

            builder.Property(p => p.Relevance)
              .IsRequired();

            builder.Property(p => p.Level)
              .IsRequired();
        }
    }
}
