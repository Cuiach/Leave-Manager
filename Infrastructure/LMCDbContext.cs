using Microsoft.EntityFrameworkCore;
using Leave_Manager_Console.Entities;
using System.Reflection.Emit;

namespace Leave_Manager_Console.Infrastructure
{
    internal class LMCDbContext : DbContext
    {
        public DbSet<Entities.Employee> Employees { get; set; }
        public DbSet<Entities.Leave> Leaves { get; set; }
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
        }
    }
}
