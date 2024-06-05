// this .cs is not used at the moment; this is only a kind of plan to rewrite code in the future

using Leave_Manager_Console.Entities;

namespace Leave_Manager_Console.Infrastructure
{
    internal class EmployeeRepository
    {
        private readonly LMCDbContext _context;

        public EmployeeRepository(LMCDbContext context)
        {
            _context = context;
        }

        public List<Employee> GetAllUsers()
        {
            return _context.Employees.ToList();
        }
    }
}
