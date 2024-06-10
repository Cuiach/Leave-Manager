using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Leave_Manager.Leave_Manager.Core.Entities;

namespace Leave_Manager.Leave_Manager.Infrastructure.Persistence
{
    internal class LMDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<LeaveLimit> LeaveLimits { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Leave_Manager_ConsoleDb;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>();
            modelBuilder.Entity<Leave>(e =>
            {
                e.HasOne<Employee>().WithMany().HasForeignKey(l => l.EmployeeId);
            });
            modelBuilder.Entity<LeaveLimit>();
        }
    }
}
