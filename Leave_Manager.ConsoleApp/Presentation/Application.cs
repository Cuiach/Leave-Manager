using Leave_Manager.Leave_Manager.Core.Interfaces;
using System.Threading.Tasks;

namespace Leave_Manager.Application
{
    public class Application
    {
        private readonly IListOfEmployeesService _listOfEmployeesService;

        public Application(IListOfEmployeesService listOfEmployeesService)
        {
            _listOfEmployeesService = listOfEmployeesService;
        }

        public async Task AddEmployeeAsync()
        {
            await _listOfEmployeesService.AddEmployeeAsync();
        }

        public async Task DisplayAllEmployeesAsync()
        {
            await _listOfEmployeesService.DisplayAllEmployeesAsync();
        }

        public async Task DisplayMatchingEmployeesAsync(string searchPhrase)
        {
            await _listOfEmployeesService.DisplayMatchingEmployeesAsync(searchPhrase);
        }

        public async Task RemoveEmployeeAsync(int intToRemove)
        {
            await _listOfEmployeesService.RemoveEmployeeAsync(intToRemove);
        }

        public async Task AddLeaveAsync(int employeeId)
        {
            await _listOfEmployeesService.AddLeaveAsync(employeeId);
        }

        public async Task DisplayAllLeavesAsync()
        {
            await _listOfEmployeesService.DisplayAllLeavesAsync();
        }

        public async Task DisplayAllLeavesOnDemandAsync()
        {
            await _listOfEmployeesService.DisplayAllLeavesOnDemandAsync();
        }

        public async Task DisplayAllLeavesForEmployeeAsync(int employeeId)
        {
            await _listOfEmployeesService.DisplayAllLeavesForEmployeeAsync(employeeId);
        }

        public async Task DisplayAllLeavesForEmployeeOnDemandAsync(int employeeId)
        {
            await _listOfEmployeesService.DisplayAllLeavesForEmployeeOnDemandAsync(employeeId);
        }

        public async Task RemoveLeaveAsync(int intOfLeaveToRemove)
        {
            await _listOfEmployeesService.RemoveLeaveAsync(intOfLeaveToRemove);
        }

        public async Task EditLeaveAsync(int intOfLeaveToEdit)
        {
            await _listOfEmployeesService.EditLeaveAsync(intOfLeaveToEdit);
        }

        public async Task EditSettingsAsync(int employeeIdToEdit)
        {
            await _listOfEmployeesService.EditSettingsAsync(employeeIdToEdit);
        }
    }
}