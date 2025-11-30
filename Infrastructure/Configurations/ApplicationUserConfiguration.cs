using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FullName)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(u => u.Address)
                  .HasMaxLength(200)
                  .IsRequired(false);

            builder.Property(u => u.Gender)
                   .HasMaxLength(10)
                   .IsRequired(false);

            builder.Property(u => u.Age)
                   .IsRequired();
        }
    }
}
