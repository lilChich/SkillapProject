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
    class TagsMapping : IEntityTypeConfiguration<Tags>
    {
        public void Configure(EntityTypeBuilder<Tags> builder)
        {
            builder.ToTable("Tags", "dbo");

            builder.HasKey(x => new { x.Id });

            builder.Property(p => p.Name)
               .HasMaxLength(30)
               .IsRequired()
               .IsUnicode();

        }
    }
}
