using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Inewi_Console.Entities
{

    public class Employee(string fName, string lName, int numberAsId)
    {
        public int Id { get; set; } = numberAsId;
        public string FirstName { get; set; } = fName;
        public string LastName { get; set; } = lName;
        public int? WorkingYears { get; set; }
        public DateTime DayOfJoining { get; set; }
        public List<LeaveLimit> LeaveLimits { get; set; } = [];
        public enum YearsToTakeLeave
        {
            CurrentOnly,
            OneMore,
            TwoMore,
            NoLimit
        }
        public YearsToTakeLeave HowManyYearsToTakePastLeave = YearsToTakeLeave.OneMore;
        public int LeavesPerYear { get; set; }
        public int OnDemandPerYear { get; set; }
        internal static void ChangeAccruedLeaveLimitPolicy(Employee employee)
        {
            var choice = "";
            string yearsWhenLeaveIsValid = "";

            switch (employee.HowManyYearsToTakePastLeave)
            {
                case Employee.YearsToTakeLeave.CurrentOnly:
                    yearsWhenLeaveIsValid = "only current year";
                    break;
                case Employee.YearsToTakeLeave.OneMore:
                    yearsWhenLeaveIsValid = "1 additional past year";
                    break;
                case Employee.YearsToTakeLeave.TwoMore:
                    yearsWhenLeaveIsValid = "2 additional past years";
                    break;
                case Employee.YearsToTakeLeave.NoLimit:
                    yearsWhenLeaveIsValid = "no cap";
                    break;
            }

            Console.WriteLine($"Accrued leave cap: {yearsWhenLeaveIsValid}. Do you want to change it? (press y if yes)");
            choice = Console.ReadLine();

            if (choice == "y")
            {
                Console.WriteLine("Choose accrued leave cap: \n0 = only current year \n1 = current year and last year \n2 = current and two last years \n3 = no cap");
                string? newLeaveYearsLimit = Console.ReadLine();
                try
                {
                    int number = int.Parse(newLeaveYearsLimit);
                    switch (newLeaveYearsLimit)
                    {
                        case "0":
                            employee.HowManyYearsToTakePastLeave = Employee.YearsToTakeLeave.CurrentOnly;
                            break;
                        case "1":
                            employee.HowManyYearsToTakePastLeave = Employee.YearsToTakeLeave.OneMore;
                            break;
                        case "2":
                            employee.HowManyYearsToTakePastLeave = Employee.YearsToTakeLeave.TwoMore;
                            break;
                        case "3":
                            employee.HowManyYearsToTakePastLeave = Employee.YearsToTakeLeave.NoLimit;
                            break;
                        default:
                            Console.WriteLine("Something went wrong - nothing set anew");
                            break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Input is not in correct format.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Input is too large or too small.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
        internal static void AdjustDateOfRecruitmentAndThusLeaveLimits(Employee employee, string newDateOfRecruitmentFromUser)
        {
            int oldYearOfRecruitment = employee.DayOfJoining.Year;
            DateTime newDateOfRecruitment = DateTime.ParseExact(newDateOfRecruitmentFromUser, "yyyy-MM-dd", null);
            employee.DayOfJoining = newDateOfRecruitment;
            Console.WriteLine($"Date of recruitment is set to: {newDateOfRecruitmentFromUser}");
            for (int i = oldYearOfRecruitment; i < newDateOfRecruitment.Year; i++)
            {
                LeaveLimit limitToRemove = employee.LeaveLimits.FirstOrDefault(l => l.Year == i);
                employee.LeaveLimits.Remove(limitToRemove);
            }
        }
        internal static void SeeAndChangeLeaveLimits(Employee employee)
        {
            var choice = "";
            Console.WriteLine("See and change leave limits for employee for given year");

            for (int i = employee.DayOfJoining.Year; i < DateTime.Now.Year; i++)
            {
                LeaveLimit leaveLimit = employee.LeaveLimits.FirstOrDefault(l => l.Year == i);
                if (leaveLimit == null)
                {
                    leaveLimit = new(i, employee.LeavesPerYear);
                    employee.LeaveLimits.Add(leaveLimit);
                }

                Console.WriteLine($"Year: {i}, limit: {leaveLimit.Limit}");
                Console.WriteLine("Do you want to change the limit? Press y if yes");
                choice = Console.ReadLine();

                if (choice == "y")
                {
                    Console.WriteLine("Put limit:");
                    string? newLimit = Console.ReadLine();
                    try
                    {
                        int number = int.Parse(newLimit);
                        leaveLimit.Limit = number;
                        Console.WriteLine($"Year: {i}, limit is set to: {number}");
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Input is not in correct format.");
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Input is too large or too small.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
        }
        internal static bool LeaveLimitForCurrentYearExists(Employee employee)
        {
            LeaveLimit leaveLimit = employee.LeaveLimits.FirstOrDefault(l => l.Year == DateTime.Now.Year);
            if (leaveLimit != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
