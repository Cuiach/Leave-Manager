using Leave_Manager.Leave_Manager.Core.Entities;
using Leave_Manager.Leave_Manager.Core.Interfaces;

namespace Leave_Manager.Leave_Manager.Infrastructure.Persistence
{
    internal class EmployeeRepository : IEmployeeRepository
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
