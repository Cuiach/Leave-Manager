using Leave_Manager.Application;
using Leave_Manager.Leave_Manager.Core.Entities;
using Leave_Manager.Leave_Manager.Core.Interfaces;
using Leave_Manager.Leave_Manager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Manager.Leave_Manager.Core.Services
{
    public class LeaveManagementService : ILeaveManagementService
    {
        private readonly LMDbContext _context;
        public List<Leave> Leaves { get; private set; } = [];

        public LeaveManagementService(LMDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null.");
            Leaves = _context.Leaves.ToList(); // You might want to make this asynchronous in the future
        }

        public async Task<List<Leave>> GetAllLeavesAsync()
        {
            var allLeaves = await _context.Leaves.ToListAsync();

            if (allLeaves.Any())
            {
                Console.WriteLine($"Leaves are transferred from database.");
            }
            else
            {
                Console.WriteLine("No leaves were found in database.");
            }
            return allLeaves;
        }

        private async Task AddLeaveLastPartAsync(Leave leave)
        {
            Leave leaveHere = new Leave()
            {
                EmployeeId = leave.EmployeeId,
                DateFrom = leave.DateFrom,
                DateTo = leave.DateTo,
                IsOnDemand = leave.IsOnDemand
            };

            await _context.Leaves.AddAsync(leaveHere);
            await _context.SaveChangesAsync();
            leave.Id = leaveHere.Id;
            _context.Entry(leaveHere).State = EntityState.Detached;

            Leaves.Add(leave);
        }

        private async Task RemoveLeaveLastPartAsync(Leave leave)
        {
            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();
            Leaves.Remove(leave);
        }

        public async Task SplitLeaveIntoConsecutiveBusinessDaysBitsAsync(Leave leave)
        {
            int leaveId = leave.Id;
            int employeeId = leave.EmployeeId;
            DateTime dateFrom = leave.DateFrom;
            DateTime dateTo = leave.DateTo;
            bool isOnDemand = leave.IsOnDemand;
            await RemoveLeaveAsync(leaveId);

            WorkDaysCalculator workDaysCalculator = new();
            List<List<DateTime>> listOfLeaves = workDaysCalculator.GetUninterruptedWorkDays(dateFrom, dateTo);

            int i = 0;
            foreach (var uninterruptedRange in listOfLeaves)
            {
                Leave leaveHere = new(employeeId, false);
                leaveHere.IsOnDemand = isOnDemand;
                leaveHere.DateFrom = uninterruptedRange.First();
                leaveHere.DateTo = uninterruptedRange.Last();

                await AddLeaveLastPartAsync(leaveHere);
                i++;
            }
        }

        public async Task<int> GetLastLeaveYearOfEmployeeAsync(int employeeId)
        {
            var leaves = await _context.Leaves
                .Where(l => l.EmployeeId == employeeId)
                .ToListAsync();

            if (leaves.Count == 0)
            {
                return 0;
            }

            DateTime lastLeaveDate = leaves.Max(l => l.DateTo);
            return lastLeaveDate.Year;
        }

        public async Task<bool> CheckOverlappingAsync(Leave leave)
        {
            var leavesOverlapping = await _context.Leaves
                .Where(l => l.EmployeeId == leave.EmployeeId)
                .Where(l => l.DateTo >= leave.DateFrom)
                .Where(l => l.DateFrom <= leave.DateTo)
                .ToListAsync();

            if (leavesOverlapping.Count > 0)
            {
                Console.Write("Overlapping: ");
                foreach (Leave l in leavesOverlapping)
                {
                    AuxiliaryMethods.DisplayLeaveDetails(l);
                }
                return false;
            }
            return true;
        }

        public async Task AddLeaveAsync(Leave leave, bool askIfOnDemand)
        {
            if (leave.DateFrom.Year == DateTime.Now.Year && askIfOnDemand)
            {
                Console.WriteLine("Is this leave On Demand? (click y to nod or n to deny or enter to skip)");

                string input = Console.ReadLine();

                _ = input == "y" ? (leave.IsOnDemand = true) : input == "n" ? leave.IsOnDemand = false : true;
            }

            await AddLeaveLastPartAsync(leave);
        }

        public async Task RemoveLeaveAsync(int intOfLeaveToRemove)
        {
            var leaveToRemove = Leaves.FirstOrDefault(c => c.Id == intOfLeaveToRemove);
            if (leaveToRemove == null)
            {
                Console.WriteLine("Leave not found");
            }
            else
            {
                await RemoveLeaveLastPartAsync(leaveToRemove);
            }
        }

        public async Task DisplayAllLeavesAsync()
        {
            var leaves = await GetAllLeavesAsync();
            AuxiliaryMethods.DisplayLeaves(leaves);
        }

        public async Task DisplayAllLeavesOnDemandAsync()
        {
            var onDemandLeaves = await _context.Leaves
                .Where(l => l.IsOnDemand)
                .ToListAsync();
            AuxiliaryMethods.DisplayLeaves(onDemandLeaves);
        }

        public async Task DisplayAllLeavesForEmployeeAsync(int employeeId)
        {
            var leavesOfEmployee = await _context.Leaves
                .Where(l => l.EmployeeId == employeeId)
                .ToListAsync();
            AuxiliaryMethods.DisplayLeaves(leavesOfEmployee);
        }

        public async Task DisplayAllLeavesForEmployeeOnDemandAsync(int employeeId)
        {
            var leavesOfEmployeeOnDemand = await _context.Leaves
                .Where(l => l.EmployeeId == employeeId)
                .Where(l => l.IsOnDemand)
                .ToListAsync();
            AuxiliaryMethods.DisplayLeaves(leavesOfEmployeeOnDemand);
        }

        public async Task<int> GetSumOfDaysOnLeaveTakenByEmployeeInYearAsync(int employeeId, int year)
        {
            var leaves = await _context.Leaves
                .Where(l => l.EmployeeId == employeeId && l.DateFrom.Year == year)
                .ToListAsync();

            return leaves.Sum(l => l.GetLeaveLength());
        }

        public async Task<int> GetSumOnDemandAsync(int employeeId)
        {
            var leaves = await _context.Leaves
                .Where(l => l.EmployeeId == employeeId && l.IsOnDemand)
                .ToListAsync();

            return leaves.Sum(l => l.GetLeaveLength());
        }

        // You can keep this synchronous for now if it's not causing performance issues.
        public int CountSumOfPastYearLeaveDays(int employeeId, int year)
        {
            int sumOfPreviousYearLeaveDays = 0;
            foreach (var leave in Leaves)
            {
                if (leave.EmployeeId == employeeId && leave.DateFrom.Year == year)
                {
                    sumOfPreviousYearLeaveDays += leave.GetLeaveLength();
                }
            }
            return sumOfPreviousYearLeaveDays;
        }
    }
}