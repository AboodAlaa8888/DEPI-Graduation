using Core.Models;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.DbContexts
{
    public class NursingServicesDbContext : IdentityDbContext<ApplicationUser>
    {
        public NursingServicesDbContext(DbContextOptions<NursingServicesDbContext> options) : base(options)
        {
        }

        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NursingServicesDbContext).Assembly);
        }
    }
}
