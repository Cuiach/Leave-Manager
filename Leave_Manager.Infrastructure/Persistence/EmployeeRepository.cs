using Leave_Manager.Leave_Manager.Core.Entities;
using Leave_Manager.Leave_Manager.Core.Interfaces;
using Leave_Manager.Leave_Manager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<int> AddEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
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