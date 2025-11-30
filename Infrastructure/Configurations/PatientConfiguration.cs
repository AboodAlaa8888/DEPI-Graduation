using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Models;


namespace Infrastructure.Configurations
{

    internal class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.UserName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(p => p.Gender)
                .HasMaxLength(10);

            builder.Property(p => p.Age)
                .IsRequired();

            builder.Property(p => p.Address)
                .HasMaxLength(200);

            builder.HasMany(p => p.Orders)
                .WithOne(o => o.Patient)
                .HasForeignKey(o => o.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
