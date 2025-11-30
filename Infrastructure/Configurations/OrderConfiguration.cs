using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> O)
        {
            O.ToTable("Orders");

            O.HasKey(o => o.Id);

            O.Property(o => o.Id)
                .UseIdentityColumn(1, 1);

            O.Property(o => o.Description)
                .IsRequired()
                .HasMaxLength(150);

            O.Property(o => o.Duration)
                  .IsRequired();
                  

            O.Property(o => o.Address)
                  .IsRequired()
                  .HasMaxLength(150);

            O.Property(o => o.PatientAge)
                  .IsRequired();

            O.Property(o => o.Status)
                  .IsRequired()
                  .HasConversion<string>()
                  .HasMaxLength(20);

            O.Property(o => o.OrderDate)
                  .IsRequired()
                  .HasColumnType("datetime");
        }
    }
}
