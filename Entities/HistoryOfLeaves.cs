using System.ComponentModel.DataAnnotations;

namespace Inewi_Console.Entities
{
    public class HistoryOfLeaves
    {
        public List<Leave> Leaves { get; set; } = [];
        public void AddLeave(int employeeId)
        {
            int lastAddedLeaveId = Leaves.Count == 0 ? 0 : Leaves.LastOrDefault().Id;

            var leave = new Leave(employeeId, lastAddedLeaveId + 1);
            Console.WriteLine("Default leave dates are set to: from {0}, to {1}. Do you want to change them? (y/n)", leave.DateFrom, leave.DateTo);
            //Console.WriteLine("Default leave dates are set to: from {0:dd-MM-yyyy}, to {1:dd-MM-yyyy}. Do you want to change them? (y/n)", leave.DateFrom, leave.DateTo);
            if (Console.ReadLine() == "y")
            {
                Console.WriteLine("Put date - beginning of leave");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime userDateTimeFrom))
                {
                    Console.WriteLine("The day of the week is: " + userDateTimeFrom.DayOfWeek);
                    leave.DateFrom = userDateTimeFrom;
                }
                else
                {
                    Console.WriteLine("You have entered an incorrect value.");
                }

                Console.WriteLine("Put date - end of leave");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime userDateTimeTo))
                {
                    Console.WriteLine("The day of the week is: " + userDateTimeTo.DayOfWeek);
                    leave.DateTo = userDateTimeTo;
                }
                else
                {
                    Console.WriteLine("You have entered an incorrect value.");
                }

            }
            Leaves.Add(leave);
        }

        private static void DisplayLeaveDetails(Leave leave)
        {
            Console.WriteLine($"Leave details Id={leave.Id}, Employee Id={leave.EmployeeId}, leave from: {leave.DateFrom}, leave to: {leave.DateTo}");
        }
        private static void DisplayAllLeavesDetails(List<Leave> leaves)
        {
            foreach (var leave in leaves)
            {
                DisplayLeaveDetails(leave);
            }
        }
        public void DisplayLeave(int number)
        {
            var leave = Leaves.FirstOrDefault(c => c.Id == number);
            if (leave == null)
            {
                Console.WriteLine("Leave not found");
            }
            else
            {
                DisplayLeaveDetails(leave);
            }
        }
        public void DisplayAllLeaves()
        {
            DisplayAllLeavesDetails(Leaves);
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
                Leaves.Remove(leaveToRemove);
            }
        }
        public void EditLeave(int intOfLeaveToEdit)
        {
            var leaveToEdit = Leaves.FirstOrDefault(c => c.Id == intOfLeaveToEdit);
            if (leaveToEdit == null)
            {
                Console.WriteLine("Leave not found");
            }
            else
            {
                Console.WriteLine("Put correct date of beginning of leave (or put any letter to skip)");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime userDateTimeFrom))
                {
                    Console.WriteLine("The day of the week is: " + userDateTimeFrom.DayOfWeek);
                    leaveToEdit.DateFrom = userDateTimeFrom;
                }
                else
                {
                    Console.WriteLine("You skipped editing date 'from'");
                }

                Console.WriteLine("Put correct date of end of leave (or put any letter to skip)");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime userDateTimeTo))
                {
                    Console.WriteLine("The day of the week is: " + userDateTimeTo.DayOfWeek);
                    leaveToEdit.DateTo = userDateTimeTo;
                }
                else
                {
                    Console.WriteLine("You skipped editing date 'to'");
                }

            }
        }
    }
}
