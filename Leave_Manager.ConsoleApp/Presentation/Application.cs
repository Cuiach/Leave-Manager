using Leave_Manager.Leave_Manager.Core.Interfaces;
using Leave_Manager.Leave_Manager.Core.Services;

namespace Leave_Manager.Leave_Manager.ConsoleApp.Presentation
{
    public class Application
    {
        private readonly ILeaveManagementService _leaveManagementService;
        private readonly IListOfEmployeesService _listOfEmployeesService;

        public Application(ILeaveManagementService leaveManagementService, IListOfEmployeesService listOfEmployeesService)
        {
            _leaveManagementService = leaveManagementService;
            _listOfEmployeesService = listOfEmployeesService;
        }

        //ListOfEmployeesService listOfEmployees = new();

        public void AddEmployee()
        {
            _listOfEmployeesService.AddEmployee();
        }

        public void DisplayAllEmployees()
        {
            _listOfEmployeesService.DisplayAllEmployees();
        }

        public void DisplayMatchingEmployees(string searchPhrase)
        {
            _listOfEmployeesService.DisplayMatchingEmployees(searchPhrase);
        }

        public void RemoveEmployee(int intToRemove)
        {
            _listOfEmployeesService.RemoveEmployee(intToRemove);
        }

        public void AddLeave(int employeeId)
        {
            _listOfEmployeesService.AddLeave(employeeId);
        }

        public void DisplayAllLeaves()
        {
            _listOfEmployeesService.DisplayAllLeaves();
        }

        public void DisplayAllLeavesOnDemand()
        {
            _listOfEmployeesService.DisplayAllLeavesOnDemand();
        }

        public void DisplayAllLeavesForEmployee(int employeeId)
        {
            _listOfEmployeesService.DisplayAllLeavesForEmployee(employeeId);
        }

        public void DisplayAllLeavesForEmployeeOnDemand(int employeeId)
        {
            _listOfEmployeesService.DisplayAllLeavesForEmployeeOnDemand(employeeId);
        }

        public void RemoveLeave(int intOfLeaveToRemove)
        {
            _listOfEmployeesService.RemoveLeave(intOfLeaveToRemove);
        }

        public void EditLeave(int intOfLeaveToEdit)
        {
            _listOfEmployeesService.EditLeave(intOfLeaveToEdit);
        }

        public void EditSettings(int EmployeeIdToEdit)
        {
            _listOfEmployeesService.EditSettings(EmployeeIdToEdit);
        }
    }
}