using Leave_Manager.Leave_Manager.Core.Entities;

namespace Leave_Manager.Leave_Manager.Core.Interfaces
{
    public interface ILeaveManagementService
    {
        List<Leave> Leaves { get; set; }

        void AddLeave(Leave leave, bool askIfOnDemand);
        bool CheckOverlapping(Leave leave);
        int CountSumOfPastYearLeaveDays(int employeeId, int year);
        void DisplayAllLeaves();
        void DisplayAllLeavesForEmployee(int employeeId);
        void DisplayAllLeavesForEmployeeOnDemand(int employeeId);
        void DisplayAllLeavesOnDemand();
        int GetSumOfDaysOnLeaveTakenByEmployeeInYear(int employeeId, int year);
        int GetSumOnDemand(int employeeId);
        void RemoveLeave(int intOfLeaveToRemove);
    }
}