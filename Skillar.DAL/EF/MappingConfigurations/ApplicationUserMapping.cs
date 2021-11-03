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
    class ApplicationUserMapping : IEntityTypeConfiguration<ApplicationUsers>
    {
        public void Configure(EntityTypeBuilder<ApplicationUsers> builder)
        {
            builder.ToTable("AspNetUsers", "dbo");

            builder.HasKey(x => new { x.Id });           
        }
    }
}
