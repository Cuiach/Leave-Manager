using Leave_Manager.Leave_Manager.Core.Entities;

namespace Leave_Manager.Leave_Manager.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        List<Employee> GetAllEmployeesSync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task UpdateEmployeeAsync(Employee employee);
    }
}