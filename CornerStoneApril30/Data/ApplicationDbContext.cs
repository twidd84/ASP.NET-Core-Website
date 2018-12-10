using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CornerStoneApril30.Models;

namespace CornerStoneApril30.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ParentAssignment> ParentAssignments { get; set; }
        public DbSet<TripAssignment> TripAssignments { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<School> School { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<Driver>().ToTable("Driver");
            builder.Entity<Parent>().ToTable("Parent");
            builder.Entity<Student>().ToTable("Student");
            builder.Entity<ParentAssignment>().ToTable("ParentAssignment");
            builder.Entity<TripAssignment>().ToTable("TripAssignment");
            builder.Entity<Trip>().ToTable("Trip");
            builder.Entity<Bus>().ToTable("Bus");
            builder.Entity<School>().ToTable("School");
        }
    }
}
