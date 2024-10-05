using Garage88.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Garage88.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Mechanic> Mechanics { get; set; }
        public DbSet<Role> MechanicsRoles { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Estimate> Estimates { get; set; }
        public DbSet<EstimateDetail> EstimateDetails { get; set; }
        public DbSet<EstimateDetailTemp> EstimateDetailTemps { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<Invoice> Invoices { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Delete All Connections using Cascade Delete Rule
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
