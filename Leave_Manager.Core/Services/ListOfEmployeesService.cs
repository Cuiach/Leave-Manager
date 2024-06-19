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
    public class ListOfEmployeesService : IListOfEmployeesService
    {
        private readonly ILeaveManagementService _leaveManagementService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly LMDbContext _context;
        private List<Employee> _employees = new List<Employee>();

        public ListOfEmployeesService(
            ILeaveManagementService leaveManagementService,
            IEmployeeRepository employeeRepository,
            ILeaveRepository leaveRepository,
            LMDbContext context)
        {
            _leaveManagementService = leaveManagementService;
            _employeeRepository = employeeRepository;
            _leaveRepository = leaveRepository;
            _context = context;
            _employees = GetAllEmployees();
        }

        // Employee-related methods
        public List<Employee> Employees => _employees;

        public List<Employee> GetAllEmployees()
        {
            var allEmployees = _employeeRepository.GetAllEmployeesSync();

            foreach (var employee in allEmployees)
            {
                employee.LeaveLimits = _context.LeaveLimits.Where(l => l.Employee == employee).ToList();
            }

            if (!allEmployees.Any())
            {
                Console.WriteLine("No employees found.");
            }
            return allEmployees;
        }

        private bool EmployeeExists(int employeeId)
        {
            Employee employee = Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                Console.WriteLine("There is no employee with this Id.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool AddLeaveLimitLastPart(Employee employee, LeaveLimit leaveLimit)
        {
            var employeeToUpdate = _context.Employees.Find(employee.Id);

            if (employeeToUpdate != null)
            {
                employeeToUpdate.LeaveLimits.Add(leaveLimit);
                _context.SaveChanges();
                return true;
            }
            else
            {
                Console.WriteLine("Employee not found in database.");
                return false;
            }
        }

        private void PropagateLeaveLimitsForCurrentYearForAllEmployees()
        {
            int thisYear = DateTime.Today.Year;
            foreach (var employee in Employees)
            {
                LeaveLimit leaveLimit = employee.LeaveLimits.FirstOrDefault(l => l.Year == thisYear);
                if (leaveLimit == null)
                {
                    LeaveLimit newLeaveLimit = new(thisYear, employee.LeavesPerYear);
                    if (AddLeaveLimitLastPart(employee, newLeaveLimit))
                    {
                        employee.LeaveLimits.Add(newLeaveLimit);
                    }
                }
            }
        }

        private void FillUpLeaveLimitsForFutureYears(Employee employee, int yearTo)
        {
            for (int i = DateTime.Today.Year; i <= yearTo; i++)
            {
                LeaveLimit newLimit = new(i, employee.LeavesPerYear);
                var limitForYear = employee.LeaveLimits.Where(l => l.Year == i).FirstOrDefault();
                if (limitForYear == null)
                {
                    if (AddLeaveLimitLastPart(employee, newLimit))
                    {
                        employee.LeaveLimits.Add(newLimit);
                    }
                }
            }
        }

        private async Task<bool> CheckIfLeaveLimitExistsForCurrentYearAsync(Employee employee)
        {
            if (!employee.LeaveLimitForCurrentYearExists())
            {
                Console.WriteLine("It looks there is one or more employees who do not have a leave limit set for this year. Do you want to fill leave limits in all missing places? Put y if yes");
                if (Console.ReadLine() == "y")
                {
                    PropagateLeaveLimitsForCurrentYearForAllEmployees();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private async Task DisplayEmployeeDetailsAsync(Employee employee, int leaveDaysTaken, int onDemandTaken)
        {
            LeaveLimit leaveLimitThisYear = employee.LeaveLimits.First(l => l.Year == DateTime.Now.Year);
            int previousYear = DateTime.Now.Year - 1;
            int leaveDaysAvailable = leaveLimitThisYear.Limit;

            if (employee.DayOfJoining.Year <= previousYear)
            {
                leaveDaysAvailable += CountExcessLeaveFromPast(employee, previousYear);
            }

            Console.Write($"Employee: {employee.FirstName}, {employee.LastName}, {employee.Id}, leaves: {leaveDaysTaken}/{leaveDaysAvailable}, " +
                          $"on demand: {onDemandTaken}/{employee.OnDemandPerYear}, joined: {employee.DayOfJoining:yyyy.MM.dd},");

            int lastLeaveYear = await _leaveManagementService.GetLastLeaveYearOfEmployeeAsync(employee.Id);
            int nowOrFutureYearIfFutureLeaveExists = DateTime.Now.Year >= lastLeaveYear ? DateTime.Now.Year : lastLeaveYear;

            for (int i = employee.DayOfJoining.Year; i <= nowOrFutureYearIfFutureLeaveExists; i++)
            {
                LeaveLimit limitPerYear = employee.LeaveLimits.FirstOrDefault(l => l.Year == i);
                Console.Write($" {i}/{limitPerYear?.Limit ?? 0}");
            }
            Console.WriteLine();
            await ShowLeaveAvailableForAllPastYearsAsync(employee);
        }

        private async Task DisplayAllEmployeesDetailsAsync(List<Employee> employees)
        {
            foreach (var employee in employees)
            {
                if (await CheckIfLeaveLimitExistsForCurrentYearAsync(employee))
                {
                    var leaveDaysTakenThisYear = await _leaveManagementService.GetSumOfDaysOnLeaveTakenByEmployeeInYearAsync(employee.Id, DateTime.Now.Year);
                    var onDemandTaken = await _leaveManagementService.GetSumOnDemandAsync(employee.Id);
                    await DisplayEmployeeDetailsAsync(employee, leaveDaysTakenThisYear, onDemandTaken);
                }
                else
                {
                    Console.WriteLine("Leave limit for employee is not set for current year. Thus, it is not possible to show details.");
                }
            }
        }

        private async Task ChangeAccruedLeaveLimitPolicyAsync(Employee employee)
        {
            var choice = "";
            string yearsWhenLeaveIsValid = "";
            Employee employeeAuxiliary = new("employee", "auxiliary", 0);
            employeeAuxiliary.HowManyYearsToTakePastLeave = employee.HowManyYearsToTakePastLeave;

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

                            if (await IsLeaveLimitPolicySatisfiedAsync(employee, 0))
                            {
                                await EditEmployeeLastPartAsync(employee);
                                Console.WriteLine("Accrued leave cap set to: current year only.");
                            }
                            else
                            {
                                employee.HowManyYearsToTakePastLeave = employeeAuxiliary.HowManyYearsToTakePastLeave;
                                Console.WriteLine("Accrued leave cap cannot be set to: current year only. Leave limit policy violated.");
                            }

                            break;

                        case "1":
                            employee.HowManyYearsToTakePastLeave = Employee.YearsToTakeLeave.OneMore;

                            if (await IsLeaveLimitPolicySatisfiedAsync(employee, 0))
                            {
                                await EditEmployeeLastPartAsync(employee);
                                Console.WriteLine("Accrued leave cap set to: 1 more past year.");
                            }
                            else
                            {
                                employee.HowManyYearsToTakePastLeave = employeeAuxiliary.HowManyYearsToTakePastLeave;
                                Console.WriteLine("Accrued leave cap cannot be set to: 1 more past year. Leave limit policy violated.");
                            }

                            break;

                        case "2":
                            employee.HowManyYearsToTakePastLeave = Employee.YearsToTakeLeave.TwoMore;

                            if (await IsLeaveLimitPolicySatisfiedAsync(employee, 0))
                            {
                                await EditEmployeeLastPartAsync(employee);
                                Console.WriteLine("Accrued leave cap set to: 2 more past years.");
                            }
                            else
                            {
                                employee.HowManyYearsToTakePastLeave = employeeAuxiliary.HowManyYearsToTakePastLeave;
                                Console.WriteLine("Accrued leave cap cannot be set to: 2 more past years. Leave limit policy violated.");
                            }

                            break;

                        case "3":
                            employee.HowManyYearsToTakePastLeave = Employee.YearsToTakeLeave.NoLimit;
                            await EditEmployeeLastPartAsync(employee);
                            Console.WriteLine("Accrued leave cap set to: no limit.");
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
            else
            {
                await EditEmployeeLastPartAsync(employee);
            }
        }

        private async Task SeeAndChangeLeaveLimitsAsync(Employee employee)
        {
            var choice = "";
            Employee employeeAuxiliary = new("employee", "auxiliary", 0);
            int latestLeaveYear = await _leaveManagementService.GetLastLeaveYearOfEmployeeAsync(employee.Id);
            int yearToMakeChangesUpTo =
                latestLeaveYear > DateTime.Now.Year ?
                latestLeaveYear : DateTime.Now.Year;
            Console.WriteLine("See and change leave limits for employee for given year");

            for (int i = employee.DayOfJoining.Year; i <= yearToMakeChangesUpTo; i++)
            {
                LeaveLimit leaveLimit = employee.LeaveLimits.FirstOrDefault(l => l.Year == i);
                if (leaveLimit == null)
                {
                    leaveLimit = new(i, employee.LeavesPerYear);
                    if (AddLeaveLimitLastPart(employee, leaveLimit))
                    {
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
                else
                {
                    Console.WriteLine($"Year: {i}, limit: {leaveLimit.Limit}");
                    Console.WriteLine("Do you want to change the limit? Press y if yes");
                    choice = Console.ReadLine();

                    if (choice == "y")
                    {
                        LeaveLimit auxiliaryLeaveLimit = new(i, leaveLimit.Limit);
                        Console.WriteLine("Put limit:");
                        string? newLimit = Console.ReadLine();
                        try
                        {
                            int number = int.Parse(newLimit);
                            leaveLimit.Limit = number;
                            if (await IsLeaveLimitPolicySatisfiedAsync(employee, await _leaveManagementService.GetLastLeaveYearOfEmployeeAsync(employee.Id)))
                            {
                                Console.WriteLine($"Year: {i}, limit is set to: {number}");
                            }
                            else
                            {
                                leaveLimit.Limit = auxiliaryLeaveLimit.Limit;
                                Console.WriteLine("Leave limit policy is violated. Leave limit was not changed.");
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
            }
        }

        private async Task AddEmployeeLastPartAsync(Employee newEmployee)
        {
            newEmployee.Id = await _employeeRepository.AddEmployeeAsync(newEmployee); // Async method
            Employees.Add(newEmployee);

            await DisplayEmployeeDetailsAsync(newEmployee, 0, 0); // Async method
            Console.WriteLine("Day of joining of employee is set to today. If you want to change it, go to edit settings");
        }

        private async Task EditEmployeeLastPartAsync(Employee oldEmployeeChanged)
        {
            await _employeeRepository.UpdateEmployeeAsync(oldEmployeeChanged);

            Console.WriteLine("Employee's details are changed");
        }

        public async Task AddEmployeeAsync()
        {
            int defaultOnDemand = 4;
            int defaultLeavePerYear = 26;
            Console.WriteLine("Insert first name");
            var firstName = Console.ReadLine();
            Console.WriteLine("Insert last name");
            var lastName = Console.ReadLine();

            if (firstName != null && lastName != null)
            {
                var newEmployee = new Employee(firstName, lastName);

                Console.WriteLine($"On Demand for the employee per year - is it {defaultOnDemand}? If yes, press enter; if not, put the correct number and enter");
                newEmployee.OnDemandPerYear = AuxiliaryMethods.SetNewLimit(Console.ReadLine(), defaultOnDemand);
                Console.WriteLine($"On-demand leave was set to: {newEmployee.OnDemandPerYear}");

                Console.WriteLine($"Leave days per year for the employee - is it {defaultLeavePerYear}? If yes, press enter; if not, put the correct number and enter");
                newEmployee.PropagateLeaveLimitForCurrentYear(AuxiliaryMethods.SetNewLimit(Console.ReadLine(), defaultLeavePerYear), true);

                newEmployee.DayOfJoining = DateTime.Today;

                await AddEmployeeLastPartAsync(newEmployee); // Use async method
            }
        }

        public async Task DisplayAllEmployeesAsync()
        {
            await DisplayAllEmployeesDetailsAsync(Employees);
        }

        public async Task RemoveEmployeeAsync(int employeeId)
        {
            if (!EmployeeExists(employeeId))
            {
                return;
            }
            else
            {
                var employeeToRemove = Employees.First(c => c.Id == employeeId);
                await _employeeRepository.DeleteEmployeeAsync(employeeId); // Async method
                Employees.Remove(employeeToRemove);
            }
        }

        public async Task DisplayMatchingEmployeesAsync(string searchPhrase)
        {
            var matchingEmployees = Employees
                .Where(e => e.FirstName == searchPhrase || e.LastName == searchPhrase)
                .ToList();

            if (matchingEmployees.Any())
            {
                await DisplayAllEmployeesDetailsAsync(matchingEmployees); // Async method
            }
            else
            {
                Console.WriteLine("No employee was found");
            }
        }

        public async Task EditSettingsAsync(int employeeIdToEdit)
        {
            if (!EmployeeExists(employeeIdToEdit))
            {
                return;
            };

            string? choice;
            var employee = Employees.FirstOrDefault(e => e.Id == employeeIdToEdit);
            int oldYearOfRecruitment = employee.DayOfJoining.Year;

            Console.WriteLine($"On Demand for the employee per year - is it {employee.OnDemandPerYear}? If yes press enter; if not put correct number and enter");
            int newOnDemand = AuxiliaryMethods.SetNewLimit(Console.ReadLine(), employee.OnDemandPerYear);
            if (await _leaveManagementService.GetSumOnDemandAsync(employee.Id) > newOnDemand)
            {
                Console.WriteLine($"On demand leave limit: {employee.OnDemandPerYear} days and cannot be changed. Check already taken leaves on demand.");
            }
            else
            {
                employee.OnDemandPerYear = newOnDemand;
                //_employeeRepository.UpdateEmployeeAsync(employee);
                Console.WriteLine($"On demand leave limit: {employee.OnDemandPerYear} days");
            }

            Console.WriteLine($"Leave days per year for the employee - is it {employee.LeavesPerYear}? If yes press enter; if not put correct number and enter");
            Employee auxiliaryEmployee = new("aux", "emp", 0);
            auxiliaryEmployee.LeavesPerYear = employee.LeavesPerYear;
            employee.PropagateLeaveLimitForCurrentYear(AuxiliaryMethods.SetNewLimit(Console.ReadLine(), employee.LeavesPerYear), false);
            if (await IsLeaveLimitPolicySatisfiedAsync(employee, await _leaveManagementService.GetLastLeaveYearOfEmployeeAsync(employeeIdToEdit)))
            {
                Console.WriteLine($"Leave limit: {employee.LeavesPerYear}");
            }
            else
            {
                employee.PropagateLeaveLimitForCurrentYear(auxiliaryEmployee.LeavesPerYear, false);
                Console.WriteLine($"Leave limit cannot be changed and is: {employee.LeavesPerYear}. Check possible violation of leave limit policy.");
            }

            Console.WriteLine("Do you want to change employee's date of recruitment? Press y if yes");
            choice = Console.ReadLine();
            if (choice == "y")
            {
                DateTime newDateOfRecruitment;
                string dateFromUser = "";
                while (!AuxiliaryMethods.IsValidDate(dateFromUser))
                {
                    Console.WriteLine("Enter a date of recruitment (yyyy-MM-dd): ");
                    dateFromUser = Console.ReadLine();
                }
                newDateOfRecruitment = DateTime.ParseExact(dateFromUser, "yyyy-MM-dd", null);

                Leave oldestLeave = (await _leaveRepository.GetAllLeavesAsync())
                    .Where(l => l.EmployeeId == employeeIdToEdit)
                    .OrderBy(l => l.DateFrom)
                    .FirstOrDefault();
                if (oldestLeave != null)
                {
                    if (oldestLeave.DateFrom < newDateOfRecruitment)
                    {
                        Console.WriteLine("There is an older leave. The employee joined earlier. So you cannot change date of recruitment.");
                    }
                    else
                    {
                        employee.AdjustDateOfRecruitmentAndThusLeaveLimits(dateFromUser);
                    }
                }
                else
                {
                    employee.AdjustDateOfRecruitmentAndThusLeaveLimits(dateFromUser);
                }
            }

            await SeeAndChangeLeaveLimitsAsync(employee); // Make async

            await ChangeAccruedLeaveLimitPolicyAsync(employee); // Make async

            await EditEmployeeLastPartAsync(employee);
        }

        // Leave-related methods 
        private async Task<bool> IsLeaveLimitPolicySatisfiedAsync(Employee employee, int yearOfLeaveEnd)
        {
            int yearToWhichCheckPolicy = yearOfLeaveEnd > DateTime.Now.Year ? yearOfLeaveEnd : DateTime.Now.Year;

            for (int i = employee.DayOfJoining.Year; i <= yearToWhichCheckPolicy; i++)
            {
                if (await CountLeaveAvailableAsync(employee, i, 0) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> IsOnDemandLimitSatisfiedAsync(Employee employee, int newOnDemandLeaveLength)
        {
            int onDemandTaken = await _leaveManagementService.GetSumOnDemandAsync(employee.Id);
            return employee.OnDemandPerYear >= onDemandTaken + newOnDemandLeaveLength;
        }

        private static bool IsLeaveAfterDateOfRecruitment(Employee employee, Leave leave)
        {
            if (leave.DateFrom < employee.DayOfJoining)
            {
                Console.WriteLine("Leave end cannot be older than the day of recruitment. Try again with correct dates.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private int ExcessLeaveFromPastYearCurrentOnly(Employee employee, int k)
        {
            return 0;
        }

        private int ExcessLeaveFromPastYearOneMore(Employee employee, int k)
        {
            LeaveLimit leaveLimitInYearK = employee.LeaveLimits.First(l => l.Year == k);
            int sumOfLeavesInYearK = _leaveManagementService.CountSumOfPastYearLeaveDays(employee.Id, k); // Keep using synchronous method

            if (employee.DayOfJoining.Year != k)
            {
                return Math.Min(leaveLimitInYearK.Limit, leaveLimitInYearK.Limit - sumOfLeavesInYearK + ExcessLeaveFromPastYearOneMore(employee, k - 1));
            }
            else
            {
                return leaveLimitInYearK.Limit - sumOfLeavesInYearK;
            }
        }

        private int ExcessLeaveFromPastYearTwoMore(Employee employee, int k)
        {
            LeaveLimit leaveLimitInYearK = employee.LeaveLimits.First(l => l.Year == k);
            int sumOfLeavesInYearK = _leaveManagementService.CountSumOfPastYearLeaveDays(employee.Id, k); // Keep using synchronous method

            if (employee.DayOfJoining.Year == k)
            {
                return leaveLimitInYearK.Limit - sumOfLeavesInYearK;
            }
            else if (employee.DayOfJoining.Year == k - 1)
            {
                return leaveLimitInYearK.Limit - sumOfLeavesInYearK + ExcessLeaveFromPastYearTwoMore(employee, k - 1);
            }
            else
            {
                LeaveLimit leaveLimitInYearPreviousToK = employee.LeaveLimits.FirstOrDefault(l => l.Year == k - 1);
                int result = Math.Min(leaveLimitInYearK.Limit + leaveLimitInYearPreviousToK.Limit, leaveLimitInYearK.Limit - sumOfLeavesInYearK + ExcessLeaveFromPastYearTwoMore(employee, k - 1));
                return result;
            }
        }

        private int ExcessLeaveFromPastYearNoLimit(Employee employee, int k)
        {
            LeaveLimit leaveLimitInYearK = employee.LeaveLimits.First(l => l.Year == k);
            int sumOfLeavesInYearK = _leaveManagementService.CountSumOfPastYearLeaveDays(employee.Id, k); // Keep using synchronous method

            if (employee.DayOfJoining.Year != k)
            {
                return leaveLimitInYearK.Limit - sumOfLeavesInYearK + ExcessLeaveFromPastYearNoLimit(employee, k - 1);
            }
            else
            {
                return leaveLimitInYearK.Limit - sumOfLeavesInYearK;
            }
        }

        private int CountExcessLeaveFromPast(Employee employee, int year)
        {
            int countedExcess = 0;
            switch (employee.HowManyYearsToTakePastLeave)
            {
                case Employee.YearsToTakeLeave.CurrentOnly:
                    countedExcess += ExcessLeaveFromPastYearCurrentOnly(employee, year);
                    break;
                case Employee.YearsToTakeLeave.OneMore:
                    countedExcess += ExcessLeaveFromPastYearOneMore(employee, year);
                    break;
                case Employee.YearsToTakeLeave.TwoMore:
                    countedExcess += ExcessLeaveFromPastYearTwoMore(employee, year);
                    break;
                case Employee.YearsToTakeLeave.NoLimit:
                    countedExcess += ExcessLeaveFromPastYearNoLimit(employee, year);
                    break;
            }
            return countedExcess;
        }

        private async Task<int> CountLeaveAvailableAsync(Employee employee, int year, int whetherToDisplayLeaveDetails)
        {
            int result = 0;
            LeaveLimit leaveLimit = employee.LeaveLimits.First(l => l.Year == year);
            result = leaveLimit.Limit - await _leaveManagementService.GetSumOfDaysOnLeaveTakenByEmployeeInYearAsync(employee.Id, year);

            if (whetherToDisplayLeaveDetails == 1)
            {
                Console.Write($"In {year} year: {leaveLimit.Limit} limit and {await _leaveManagementService.GetSumOfDaysOnLeaveTakenByEmployeeInYearAsync(employee.Id, year)} taken.");
            }

            if (employee.DayOfJoining.Year < year)
            {
                result += CountExcessLeaveFromPast(employee, year - 1);
            }
            return result;
        }

        private async Task ShowLeaveAvailableForAllPastYearsAsync(Employee employee)
        {
            int startingYear = employee.DayOfJoining.Year;
            int endingYear =
                (await _leaveManagementService.GetLastLeaveYearOfEmployeeAsync(employee.Id)) <= DateTime.Now.Year ?
                DateTime.Now.Year : await _leaveManagementService.GetLastLeaveYearOfEmployeeAsync(employee.Id);

            Console.WriteLine("TEST PURPOSE:");

            for (int i = startingYear; i <= endingYear; i++)
            {
                int k = await CountLeaveAvailableAsync(employee, i, 1);
                Console.WriteLine($" Accrued leave available in {i}: {k}");
            }
        }

        private async Task<bool> GetNewDatesOfLeaveAsync(Leave leave, Employee employee, bool totallyNewLeave)
        {
            Console.WriteLine(totallyNewLeave
                ? $"Default leave dates: from {leave.DateFrom}, to {leave.DateTo}. Put n if you want to set the dates manually"
                : "");

            if (totallyNewLeave == false || (totallyNewLeave == true && Console.ReadLine() == "n"))
            {
                Console.WriteLine("Put date of beginning of leave (or put any letter to skip)");
                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime userDateTimeFrom))
                {
                    Console.WriteLine("The day of the week is: " + userDateTimeFrom.DayOfWeek);
                    leave.DateFrom = userDateTimeFrom;
                }
                else
                {
                    Console.WriteLine(totallyNewLeave ? "You entered an incorrect value." : "Beginning of leave is not changed.");
                }

                Console.WriteLine("Put date of end of leave (or put any letter to skip)");
                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime userDateTimeTo))
                {
                    Console.WriteLine("The day of the week is: " + userDateTimeTo.DayOfWeek);
                    leave.DateTo = userDateTimeTo;
                }
                else
                {
                    Console.WriteLine("End of leave is not changed.");
                }
            }

            if (!IsLeaveAfterDateOfRecruitment(employee, leave))
            {
                Console.WriteLine("Leave cannot start before employee joined.");
                return false;
            }

            if (leave.DateFrom > leave.DateTo)
            {
                Console.WriteLine("Leave cannot end before it begins.");
                return false;
            }

            if (leave.HowManyCalendarYearsLeaveSpans() > 2)
            {
                Console.WriteLine("Leave cannot be added. Leave can span up to 2 calendar years. If you have more days off, please split your leave into smaller parts.");
                return false;
            }

            if (leave.DateTo.Year > DateTime.Today.Year)
            {
                FillUpLeaveLimitsForFutureYears(employee, leave.DateTo.Year);
            }

            //// *** Asynchronous Calls! ***
            //if (!await CheckOverlappingAsync(leave))
            //{
            //    Console.WriteLine("Leave cannot be added. Try again with correct dates.");
            //    return false;
            //}

            return true;
        }

        public async Task AddLeaveAsync(int employeeId)
        {
            if (!EmployeeExists(employeeId))
            {
                return;
            };

            Employee employee = Employees.FirstOrDefault(e => e.Id == employeeId);

            int lastAddedLeaveId = (await _leaveRepository.GetAllLeavesAsync()).Count == 0 ?
                                    0 : (await _leaveRepository.GetAllLeavesAsync()).LastOrDefault()?.Id ?? 0;
            Leave leave = new(employeeId, lastAddedLeaveId + 1, true);

            if (!await GetNewDatesOfLeaveAsync(leave, employee, true)) 
            {
                return;
            }

            if (!await _leaveManagementService.CheckOverlappingAsync(leave))
            {
                Console.WriteLine("Leave cannot be added. Try again with correct dates.");
                return;
            }

            if (leave.HowManyCalendarYearsLeaveSpans() == 1)
            {
                bool ableOnDemand = await IsOnDemandLimitSatisfiedAsync(employee, leave.GetLeaveLength());

                await _leaveManagementService.AddLeaveAsync(leave, ableOnDemand);
                await _leaveManagementService.SplitLeaveIntoConsecutiveBusinessDaysBitsAsync(leave);

                //ShowLeaveAvailableForAllPastYears(employee); //TEST PURPOSE - consider if you still need this

                if (!await IsLeaveLimitPolicySatisfiedAsync(employee, leave.DateTo.Year))
                {
                    int nowLastAddedLeaveId = (await _leaveRepository.GetAllLeavesAsync()).Count == 0 ?
                                            0 : (await _leaveRepository.GetAllLeavesAsync()).Last().Id;
                    for (int i = leave.Id; i <= nowLastAddedLeaveId; i++)
                    {
                        await _leaveManagementService.RemoveLeaveAsync(i);
                    }
                    Console.WriteLine("Leave is not added. Leave limit policy is violated.");
                }
            }
            else
            {
                Leave firstLeave = new(employee.Id, lastAddedLeaveId + 1, true);
                firstLeave.DateFrom = leave.DateFrom;
                firstLeave.DateTo = new DateTime(firstLeave.DateFrom.Year, 12, 31);

                bool firstLeaveAbleOnDemand = await IsOnDemandLimitSatisfiedAsync(employee, firstLeave.GetLeaveLength());

                await _leaveManagementService.AddLeaveAsync(firstLeave, firstLeaveAbleOnDemand);
                await _leaveManagementService.SplitLeaveIntoConsecutiveBusinessDaysBitsAsync(firstLeave);

                int secondLeaveLastAddedLeaveId = (await _leaveRepository.GetAllLeavesAsync()).Count == 0 ?
                                                   0 : (await _leaveRepository.GetAllLeavesAsync()).LastOrDefault()?.Id ?? 0;
                Leave secondLeave = new(employee.Id, secondLeaveLastAddedLeaveId + 1, true);
                secondLeave.DateTo = leave.DateTo;
                secondLeave.DateFrom = new DateTime(secondLeave.DateTo.Year, 1, 1);

                bool secondLeaveAbleOnDemand = await IsOnDemandLimitSatisfiedAsync(employee, secondLeave.GetLeaveLength());

                await _leaveManagementService.AddLeaveAsync(secondLeave, secondLeaveAbleOnDemand);
                await _leaveManagementService.SplitLeaveIntoConsecutiveBusinessDaysBitsAsync(secondLeave);

                //ShowLeaveAvailableForAllPastYears(employee); //TEST PURPOSE - consider if you still need this

                if (!await IsLeaveLimitPolicySatisfiedAsync(employee, leave.DateTo.Year))
                {
                    int nowLastAddedLeaveId = (await _leaveRepository.GetAllLeavesAsync()).Count == 0 ?
                                            0 : (await _leaveRepository.GetAllLeavesAsync()).Last().Id;
                    for (int i = leave.Id; i <= nowLastAddedLeaveId; i++)
                    {
                        await _leaveManagementService.RemoveLeaveAsync(i);
                    }
                    Console.WriteLine("Leave is not added. Leave limit policy is violated.");
                }
            }
        }

        public async Task DisplayAllLeavesAsync()
        {
            await _leaveManagementService.DisplayAllLeavesAsync();
        }

        public async Task DisplayAllLeavesOnDemandAsync()
        {
            await _leaveManagementService.DisplayAllLeavesOnDemandAsync();
        }

        public async Task DisplayAllLeavesForEmployeeAsync(int employeeId)
        {
            await _leaveManagementService.DisplayAllLeavesForEmployeeAsync(employeeId);
        }

        public async Task DisplayAllLeavesForEmployeeOnDemandAsync(int employeeId)
        {
            await _leaveManagementService.DisplayAllLeavesForEmployeeOnDemandAsync(employeeId);
        }

        public async Task RemoveLeaveAsync(int leaveIdToRemove)
        {
            Leave leave = (await _leaveRepository.GetAllLeavesAsync()).FirstOrDefault(l => l.Id == leaveIdToRemove);

            if (leaveIdToRemove == null)
            {
                Console.WriteLine("Leave not found");
                return;
            }

            await _leaveManagementService.RemoveLeaveAsync(leaveIdToRemove);
        }

        public async Task EditLeaveAsync(int intOfLeaveToEdit)
        {
            var leaveToEdit = (await _leaveRepository.GetAllLeavesAsync()).FirstOrDefault(l => l.Id == intOfLeaveToEdit);
            int leaveIdAfterEdit = (await _leaveRepository.GetAllLeavesAsync()).Count > 0 ?
                                   (await _leaveRepository.GetAllLeavesAsync()).Last().Id + 1 : 1;

            if (leaveToEdit == null)
            {
                Console.WriteLine("Leave not found");
                return;
            }

            Employee employee = Employees.First(e => e.Id == leaveToEdit.EmployeeId);
            Leave leaveAuxiliary = new(leaveToEdit.EmployeeId, leaveIdAfterEdit, true)
            {
                DateFrom = leaveToEdit.DateFrom,
                DateTo = leaveToEdit.DateTo,
                IsOnDemand = leaveToEdit.IsOnDemand
            };

            if (!await GetNewDatesOfLeaveAsync(leaveAuxiliary, employee, false))
            {
                return;
            }

            await _leaveManagementService.RemoveLeaveAsync(intOfLeaveToEdit);

            if (!await _leaveManagementService.CheckOverlappingAsync(leaveAuxiliary))
            {
                await _leaveManagementService.AddLeaveAsync(leaveToEdit, false);
                Console.WriteLine("Leave cannot be added. Try again with correct dates.");
                return;
            }

            if (leaveAuxiliary.HowManyCalendarYearsLeaveSpans() != 1)
            {
                Console.WriteLine("Please, edit leave only within one calendar year. Consider adding a new leave for another year.");
                return;
            }

            bool ableOnDemand = await IsOnDemandLimitSatisfiedAsync(employee, leaveAuxiliary.GetLeaveLength());

            if (leaveToEdit.IsOnDemand && ableOnDemand == false)
            {
                Console.WriteLine("The edited leave is On Demand. Yet after change it will not be possible due to exceeding On Demand leave limit per year. Do you want to proceed and keep the leave as NOT On Demand? (put y if yes)");
                if (Console.ReadLine() == "y")
                {
                    leaveAuxiliary.IsOnDemand = false;
                    await _leaveManagementService.AddLeaveAsync(leaveAuxiliary, ableOnDemand);
                    await _leaveManagementService.SplitLeaveIntoConsecutiveBusinessDaysBitsAsync(leaveAuxiliary);
                    //ShowLeaveAvailableForAllPastYears(employee); //TEST PURPOSE

                    if (!await IsLeaveLimitPolicySatisfiedAsync(employee, leaveAuxiliary.DateTo.Year))
                    {
                        int lastLeaveIdNow = (await _leaveRepository.GetAllLeavesAsync()).Last().Id;
                        for (int i = leaveIdAfterEdit; i <= lastLeaveIdNow; i++)
                        {
                            await _leaveManagementService.RemoveLeaveAsync(i);
                        }

                        await _leaveManagementService.AddLeaveAsync(leaveToEdit, false);
                        Console.WriteLine("Leave cannot be changed. Leave limit policy is violated.");
                    }
                }
                else
                {
                    await _leaveManagementService.AddLeaveAsync(leaveToEdit, false);
                    Console.WriteLine("Leave is not changed.");
                }
            }
            else
            {
                await _leaveManagementService.AddLeaveAsync(leaveAuxiliary, ableOnDemand);
                await _leaveManagementService.SplitLeaveIntoConsecutiveBusinessDaysBitsAsync(leaveAuxiliary);
                //ShowLeaveAvailableForAllPastYears(employee); //TEST PURPOSE

                if (!await IsLeaveLimitPolicySatisfiedAsync(employee, leaveAuxiliary.DateTo.Year))
                {
                    int lastLeaveIdNow = (await _leaveRepository.GetAllLeavesAsync()).Last().Id;
                    for (int i = leaveIdAfterEdit; i <= lastLeaveIdNow; i++)
                    {
                        await _leaveManagementService.RemoveLeaveAsync(i);
                    }

                    await _leaveManagementService.AddLeaveAsync(leaveToEdit, false);
                    Console.WriteLine("Leave cannot be changed. Leave limit policy is violated.");
                }
            }
        }
    }
}