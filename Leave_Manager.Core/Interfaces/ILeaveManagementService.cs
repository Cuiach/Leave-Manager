using Leave_Manager.Leave_Manager.Core.Entities;

namespace Leave_Manager.Leave_Manager.Core.Interfaces
{
    public interface ILeaveManagementService
    {
        List<Leave> Leaves { get; }

        Task AddLeaveAsync(Leave leave, bool askIfOnDemand);
        Task<bool> CheckOverlappingAsync(Leave leave);
        int CountSumOfPastYearLeaveDays(int employeeId, int year);
        Task DisplayAllLeavesAsync();
        Task DisplayAllLeavesForEmployeeAsync(int employeeId);
        Task DisplayAllLeavesForEmployeeOnDemandAsync(int employeeId);
        Task DisplayAllLeavesOnDemandAsync();
        Task<List<Leave>> GetAllLeavesAsync();
        Task<int> GetLastLeaveYearOfEmployeeAsync(int employeeId);
        Task<int> GetSumOfDaysOnLeaveTakenByEmployeeInYearAsync(int employeeId, int year);
        Task<int> GetSumOnDemandAsync(int employeeId);
        Task RemoveLeaveAsync(int intOfLeaveToRemove);
        Task SplitLeaveIntoConsecutiveBusinessDaysBitsAsync(Leave leave);
    }
}