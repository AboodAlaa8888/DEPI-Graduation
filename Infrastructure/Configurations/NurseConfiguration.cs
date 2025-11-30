using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class NurseConfiguration : IEntityTypeConfiguration<Nurse>
    {
        public void Configure(EntityTypeBuilder<Nurse> N)
        {
            N.ToTable("Nurses");


            N.HasKey(n => n.Id);

            N.Property(n => n.Id)
                  .UseIdentityColumn(1, 1);

            N.Property(n => n.FullName)
                  .IsRequired()
                  .HasMaxLength(100);

            N.Property(n => n.Age)
                  .IsRequired();

            N.Property(n => n.Gender)
                  .IsRequired()
                  .HasMaxLength(10);

            N.Property(n => n.Experience_years)
                  .IsRequired();

            N.Property(n => n.Address)
                  .HasMaxLength(200);

            N.Property(n => n.Description)
                  .HasMaxLength(500)
                  .IsRequired();

            N.Property(n => n.PictureUrl)
                  .HasMaxLength(255)
                  .IsRequired(false);

            N.HasMany(n => n.Orders)
                  .WithOne(o => o.Nurse)
                  .HasForeignKey(o => o.NurseId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
