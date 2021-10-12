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
    class Post_TagsMapping : IEntityTypeConfiguration<Post_Tags>
    {
        public void Configure(EntityTypeBuilder<Post_Tags> builder)
        {
            builder.ToTable("PostTags", "dbo");

            builder.HasKey(x => new { x.Id });

            builder.HasOne(x => x.Post)
                .WithMany(x => x.PostTags)
                .HasForeignKey(x => x.PostId);

            builder.HasOne(x => x.Tag)
                .WithMany(x => x.PostTags)
                .HasForeignKey(x => x.TagId);
        }
    }
}
