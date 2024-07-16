using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Preventyon.Models;
using System;
using System.Reflection.Emit;
namespace Preventyon.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> dbContextOptions) : base(dbContextOptions)
        {


        }
        public DbSet<Incident> Incident { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Role { get; set; } 
        public DbSet <Permission> Permission { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<Role>()
                .HasOne(r => r.Permission) 
                .WithOne(p => p.Role) 
                .HasForeignKey<Role>(r => r.PermissionID);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role) 
                .WithMany(r => r.Employees) 
                .HasForeignKey(e => e.RoleId);

        }
    }
}
