using Leave_Manager.Leave_Manager.Core.Entities;

namespace Leave_Manager.Leave_Manager.Core.Interfaces
{
    public interface IListOfEmployeesService
    {
        List<Employee> Employees { get; }

        void AddEmployee();
        void AddLeave(int employeeId);
        void DisplayAllEmployees();
        void DisplayAllLeaves();
        void DisplayAllLeavesForEmployee(int employeeId);
        void DisplayAllLeavesForEmployeeOnDemand(int employeeId);
        void DisplayAllLeavesOnDemand();
        void DisplayMatchingEmployees(string searchPhrase);
        void EditLeave(int intOfLeaveToEdit);
        void EditSettings(int employeeIdToEdit);
        List<Employee> GetAllEmployees();
        void RemoveEmployee(int employeeId);
        void RemoveLeave(int leaveIdToRemove);
    }
}