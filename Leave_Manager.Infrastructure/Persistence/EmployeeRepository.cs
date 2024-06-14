using Leave_Manager.Leave_Manager.Core.Entities;
using Leave_Manager.Leave_Manager.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Leave_Manager.Leave_Manager.Infrastructure.Persistence
{
    internal class EmployeeRepository : IEmployeeRepository
    {
        private readonly LMDbContext _context;

        public EmployeeRepository(LMDbContext context)
        {
            _context = context;
        }

        public List<Employee> GetAllEmployeesSync()
        {
            return _context.Employees.ToList();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public int AddEmployeeSync(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
            return employee.Id;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
    }
}
