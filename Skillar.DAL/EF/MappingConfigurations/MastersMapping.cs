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
    class MastersMapping : IEntityTypeConfiguration<Masters>
    {
        public void Configure(EntityTypeBuilder<Masters> builder)
        {
            builder.ToTable("Masters", "dbo");

            builder.HasKey(x => new { x.Id });

            builder.Property(p => p.Status)
              .IsRequired();

            builder.Property(p => p.SkillLevel)
              .IsRequired();

            builder.HasOne(x => x.ApplicationUser)
               .WithMany(x => x.Masters)
               .HasForeignKey(x => x.ApplicationUserId);

            builder.HasOne(x => x.MasterClass)
                .WithMany(x => x.Masters)
                .HasForeignKey(x => x.MasterClassId);
        }
    }
}
