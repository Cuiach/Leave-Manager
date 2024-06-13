using Leave_Manager.Application;
using Leave_Manager.Leave_Manager.Core.Entities;
using Leave_Manager.Leave_Manager.Core.Interfaces;
using Leave_Manager.Leave_Manager.Infrastructure.Persistence;

namespace Leave_Manager.Leave_Manager.Core.Services
{
    public class LeaveManagementService : ILeaveManagementService
    {
        private readonly LMDbContext _context;
        public List<Leave> Leaves { get; private set; } = [];

        public LeaveManagementService(LMDbContext context)
        {
            // Check if 'context' is null and throw an exception
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "Database context cannot be null.");
            }

            _context = context;
            Leaves = _context.Leaves.ToList();
        }

        //public List<Leave> GetAllLeaves()
        //{
        //    return _context.Leaves.ToList();
        //}


        //        public LeaveManagementService(LMDbContext context)
        //        {
        //            if (context == null)
        //            {
        //                throw new ArgumentNullException(nameof(context), "Database context cannot be null.");
        //            }

        //            _context = context;
        //            Leaves = _context.Leaves.ToList();
        ////            Leaves = GetAllLeaves();
        //        }

        public List<Leave> GetAllLeaves()
        {
            var allLeaves = _context.Leaves.ToList();

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

        private void AddLeaveLastPart(Leave leave)
        {
            Leave leaveHere;
            int newLeaveId;
            leaveHere = new Leave()
            {
                EmployeeId = leave.EmployeeId,
                DateFrom = leave.DateFrom,
                DateTo = leave.DateTo,
                IsOnDemand = leave.IsOnDemand
            };

            _context.Leaves.Add(leaveHere);
            _context.SaveChanges();

            newLeaveId = leaveHere.Id;
            leave.Id = newLeaveId;

            Leaves.Add(leave);
        }

        private void RemoveLeaveLastPart(Leave leave)
        {
            _context.Leaves.Remove(leave);
            _context.SaveChanges();
            Leaves.Remove(leave);
        }

        internal void SplitLeaveIntoConsecutiveBusinessDaysBits(Leave leave)
        {
            int leaveId = leave.Id;
            int employeeeId = leave.EmployeeId;
            DateTime dateFrom = leave.DateFrom;
            DateTime dateTo = leave.DateTo;
            bool isOnDemand = leave.IsOnDemand;
            RemoveLeave(leaveId);
            WorkDaysCalculator workDaysCalculator = new();
            List<List<DateTime>> listOfLeaves = workDaysCalculator.GetUninterruptedWorkDays(dateFrom, dateTo);

            int i = 0;
            foreach (var uninterruptedRange in listOfLeaves)
            {
                Leave leaveHere = new(employeeeId, false);
                leaveHere.IsOnDemand = isOnDemand;
                leaveHere.DateFrom = uninterruptedRange.First();
                leaveHere.DateTo = uninterruptedRange.Last();

                AddLeaveLastPart(leaveHere);
                i++;
            }
        }

        internal int GetLastLeaveYearOfEmployee(int employeeId)
        {
            int year;

            var leaves = Leaves.Where(l => l.EmployeeId == employeeId).ToList();

            if (leaves.Count == 0)
            {
                year = 0;
            }
            else
            {
                DateTime lastLeaveDate = leaves.Max(l => l.DateTo);
                year = lastLeaveDate.Year;
            }

            return year;
        }

        public bool CheckOverlapping(Leave leave)
        {
            List<Leave> leavesOverlapping = Leaves.Where
                (l => l.EmployeeId == leave.EmployeeId).Where
                (l => l.DateTo >= leave.DateFrom).Where
                (l => l.DateFrom <= leave.DateTo).ToList();

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

        public void AddLeave(Leave leave, bool askIfOnDemand)
        {
            if (leave.DateFrom.Year == DateTime.Now.Year && askIfOnDemand == true)
            {
                Console.WriteLine("Is this leave On Demand? (click y to nod or n to deny or enter to skip)");

                string input = Console.ReadLine();

                bool _ = input == "y" ? (leave.IsOnDemand = true) : input == "n" ? leave.IsOnDemand = false : true;
            }

            AddLeaveLastPart(leave);
        }

        public void RemoveLeave(int intOfLeaveToRemove)
        {
            var leaveToRemove = Leaves.FirstOrDefault(c => c.Id == intOfLeaveToRemove);
            if (leaveToRemove == null)
            {
                Console.WriteLine("Leave not found");
            }
            else
            {
                RemoveLeaveLastPart(leaveToRemove);
            }
        }

        public void DisplayAllLeaves()
        {
            AuxiliaryMethods.DisplayLeaves(Leaves);
        }

        public void DisplayAllLeavesOnDemand()
        {
            var onDemandLeaves = Leaves.Where(l => l.IsOnDemand).ToList();
            AuxiliaryMethods.DisplayLeaves(onDemandLeaves);
        }

        public void DisplayAllLeavesForEmployee(int employeeId)
        {
            var leavesOfEmployee = Leaves.Where(l => l.EmployeeId == employeeId).ToList();
            AuxiliaryMethods.DisplayLeaves(leavesOfEmployee);
        }

        public void DisplayAllLeavesForEmployeeOnDemand(int employeeId)
        {
            var leavesOfEmployeeOnDemand = Leaves.Where
                (l => l.EmployeeId == employeeId).Where
                (l => l.IsOnDemand).ToList();
            AuxiliaryMethods.DisplayLeaves(leavesOfEmployeeOnDemand);
        }

        public int GetSumOfDaysOnLeaveTakenByEmployeeInYear(int employeeId, int year)
        {
            var sumOfLeaveDays = 0;
            foreach (var leave in Leaves)
            {
                if (leave.EmployeeId == employeeId && leave.DateFrom.Year == year)
                {
                    sumOfLeaveDays += leave.GetLeaveLength();
                }
            }
            return sumOfLeaveDays;
        }

        public int GetSumOnDemand(int employeeId)
        {
            var sumOfOnDemandDays = 0;
            foreach (Leave leave in Leaves)
            {
                if (leave.EmployeeId == employeeId && leave.DateFrom.Year == DateTime.Now.Year && leave.IsOnDemand == true)
                {
                    sumOfOnDemandDays += leave.GetLeaveLength();
                }
            }
            return sumOfOnDemandDays;
        }

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