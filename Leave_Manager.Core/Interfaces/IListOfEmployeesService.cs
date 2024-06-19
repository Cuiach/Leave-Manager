using Leave_Manager.Leave_Manager.Core.Entities;

namespace Leave_Manager.Leave_Manager.Core.Interfaces
{
    public interface IListOfEmployeesService
    {
        List<Employee> Employees { get; }

        Task AddEmployeeAsync();
        Task AddLeaveAsync(int employeeId);
        Task DisplayAllEmployeesAsync();
        Task DisplayAllLeavesAsync();
        Task DisplayAllLeavesForEmployeeAsync(int employeeId);
        Task DisplayAllLeavesForEmployeeOnDemandAsync(int employeeId);
        Task DisplayAllLeavesOnDemandAsync();
        Task DisplayMatchingEmployeesAsync(string searchPhrase);
        Task EditLeaveAsync(int intOfLeaveToEdit);
        Task EditSettingsAsync(int employeeIdToEdit);
        List<Employee> GetAllEmployees();
        Task RemoveEmployeeAsync(int employeeId);
        Task RemoveLeaveAsync(int leaveIdToRemove);
    }
}