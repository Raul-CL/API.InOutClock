using API.InOutClock.Shared;
using Microsoft.EntityFrameworkCore;



namespace API.InOutClock.Data
{
    public class APIInOutClockContext : DbContext
    {
        public APIInOutClockContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Check> Checks { get; set; }
        public DbSet<Shift> Shifts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer();
        }
    }
}
