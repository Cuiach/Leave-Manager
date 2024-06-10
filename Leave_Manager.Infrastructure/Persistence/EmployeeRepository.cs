// this .cs is not used at the moment; this is only a kind of plan to rewrite code in the future

using Leave_Manager.Leave_Manager.Core.Entities;

namespace Leave_Manager.Leave_Manager.Infrastructure.Persistence
{
    internal class EmployeeRepository
    {
        private readonly LMDbContext _context;

        public EmployeeRepository(LMDbContext context)
        {
            _context = context;
        }

        public List<Employee> GetAllUsers()
        {
            return _context.Employees.ToList();
        }
    }
}
