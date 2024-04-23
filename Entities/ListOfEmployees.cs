namespace Inewi_Console.Entities
{
    public class ListOfEmployees
    {
        public ListOfEmployees()
        {
            HistoryOfLeaves allLeavesInStorage = new();
            this.allLeavesInStorage = allLeavesInStorage;
        }
        public List<Employee> Employees { get; set; } = [];
        private HistoryOfLeaves allLeavesInStorage;
        //employee-related methods
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
        private void PropagateLeaveLimitsForCurrentYearForAllEmployees()
        {
            foreach (var employee in Employees)
            {
                LeaveLimit leaveLimit = employee.LeaveLimits.FirstOrDefault(l => l.Year == DateTime.Now.Year);
                if (leaveLimit == null)
                {
                    LeaveLimit newLeaveLimit = new(DateTime.Now.Year, employee.LeavesPerYear);
                    employee.LeaveLimits.Add(newLeaveLimit);
                }
            }
        }
        private bool CheckIfLeaveLimitExistsForCurrentYear(Employee employee)
        {
            if(!Employee.LeaveLimitForCurrentYearExists(employee))
            {
                Console.WriteLine("It looks there is one or more employees who do not have leave limit set for this year. Do you want to fill leave limits in all missing places? Put y if yes");
                if(Console.ReadLine() == "y")
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
        private void DisplayEmployeeDetails(Employee employee, int leaveDaysTaken, int onDemandTaken)
        {
            LeaveLimit leaveLimitThisYear = employee.LeaveLimits.First(l => l.Year == DateTime.Now.Year);
            int previousYear = DateTime.Now.Year - 1;
            int leaveDaysAvailable = leaveLimitThisYear.Limit;

            if (employee.DayOfJoining.Year <= previousYear)
            {
                leaveDaysAvailable += CountExcessLeaveFromPast(employee, previousYear);
            }

            Console.Write($"Employee: {employee.FirstName}, {employee.LastName}, {employee.Id}, leaves: {leaveDaysTaken}/{leaveDaysAvailable}, " +
                $"on demand: {onDemandTaken}/{employee.OnDemandPerYear}, joined: {employee.DayOfJoining.Year}.{employee.DayOfJoining.Month}.{employee.DayOfJoining.Day},");
            for (int i = employee.DayOfJoining.Year; i <= DateTime.Now.Year; i++)
            {
                LeaveLimit limitPerYear = employee.LeaveLimits.FirstOrDefault(l => l.Year == i);
                Console.Write($" {i}/{limitPerYear.Limit}");
            }
            Console.WriteLine();
            ShowLeaveAvailableForAllPastYears(employee);
        }
        private void DisplayAllEmployeesDetails(List<Employee> employees)
        {
            foreach (var employee in employees)
            {
                if (CheckIfLeaveLimitExistsForCurrentYear(employee))
                {
                    var leaveDaysTakenThisYear = allLeavesInStorage.GetSumOfDaysOnLeaveTakenByEmployeeInYear(employee.Id, DateTime.Now.Year);
                    var onDemandTaken = allLeavesInStorage.GetSumOnDemand(employee.Id);
                    DisplayEmployeeDetails(employee, leaveDaysTakenThisYear, onDemandTaken);
                }
                else
                {
                    Console.WriteLine("Leave limit for employee is not set for current year. Thus it is not possible to show details.");
                }
            }
        }
        public void AddEmployee()
        {
            Console.WriteLine("Insert first name");
            var firstName = Console.ReadLine();
            Console.WriteLine("Insert last name");
            var lastName = Console.ReadLine();
            
            if (firstName != null && lastName != null)
            {
                int newEmployeeId = Employees.Count == 0 ? 1 : Employees.LastOrDefault().Id + 1;

                var newEmployee = new Employee(firstName, lastName, newEmployeeId);

                Console.WriteLine("On Demand for the employee per year - is it 4? If yes press enter; if not put correct number and enter");
                var onDemandPerYear = Console.ReadLine();
                newEmployee.OnDemandPerYear = onDemandPerYear == "" ? 4 : AuxiliaryMethods.ToInt(onDemandPerYear);

                Console.WriteLine("Leave days per year for the employee - is it 26? If yes press enter; if not put correct number and enter");
                var leavesPerYearString = Console.ReadLine();
                int leavesPerYear;
                _ = leavesPerYearString == "" ? leavesPerYear = 26 : leavesPerYear = AuxiliaryMethods.ToInt(leavesPerYearString);
                newEmployee.LeavesPerYear = leavesPerYearString == "" ? 26 : leavesPerYear;

                int currentYear = DateTime.Now.Year;
                LeaveLimit leavelimit = new(currentYear, leavesPerYear);
                newEmployee.LeaveLimits.Add(leavelimit);
                newEmployee.DayOfJoining = DateTime.Today;

                Employees.Add(newEmployee);
                Console.WriteLine("Day of joing of employee is set to today. If you want to change it go to edit settings");
            }
        }
        public void DisplayAllEmployees()
        {
            DisplayAllEmployeesDetails(Employees);
        }
        public void RemoveEmployee(int employeeId)
        {
            if (!EmployeeExists(employeeId))
            {
                return;
            }
            else
            {
                var employeeToRemove = Employees.First(c => c.Id == employeeId);
                Employees.Remove(employeeToRemove);
            }
        }
        public void DisplayMatchingEmployees(string searchPhrase)
        {
            var matchingEmployees = Employees
                .Where(e => e.FirstName == searchPhrase || e.LastName == searchPhrase)
                .ToList();

            if (matchingEmployees.Any())
            {
                DisplayAllEmployeesDetails(matchingEmployees);
            }
            else
            {
                Console.WriteLine("No employee was found");
            }
        }
        public void EditSettings(int employeeIdToEdit)
        {
            if (!EmployeeExists(employeeIdToEdit))
            {
                return;
            };

            string? choice;
            var employee = Employees.FirstOrDefault(e => e.Id == employeeIdToEdit);
            int oldYearOfRecruitment = employee.DayOfJoining.Year;

            Console.WriteLine($"On Demand for the employee per year - is it {employee.OnDemandPerYear}? If yes press enter; if not put correct number and enter");
            var onDemandPerYear = Console.ReadLine();
            employee.OnDemandPerYear = onDemandPerYear == "" ? employee.OnDemandPerYear : AuxiliaryMethods.ToInt(onDemandPerYear);

            Console.WriteLine($"Leave days per year for the employee - is it {employee.LeavesPerYear}? If yes press enter; if not put correct number and enter");
            var leavesPerYear = Console.ReadLine();
            employee.LeavesPerYear = leavesPerYear == "" ? employee.LeavesPerYear : AuxiliaryMethods.ToInt(leavesPerYear);
            LeaveLimit leaveLimitThisYear = employee.LeaveLimits.FirstOrDefault(l => l.Year == DateTime.Now.Year);
            if (leaveLimitThisYear == null)
            {
                leaveLimitThisYear = new(DateTime.Now.Year, employee.LeavesPerYear);
                employee.LeaveLimits.Add(leaveLimitThisYear);
            }
            else
            {
                leaveLimitThisYear.Limit = AuxiliaryMethods.ToInt(leavesPerYear);
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

                Leave oldestLeave = allLeavesInStorage.Leaves.OrderBy(l => l.DateFrom).FirstOrDefault();
                if (oldestLeave != null)
                {
                    if (oldestLeave.DateFrom < newDateOfRecruitment)
                    {
                        Console.WriteLine("There is an older leave. The employee joined earlier. So you cannot change date of recruitment.");
                    }
                    else
                    {
                        Employee.AdjustDateOfRecruitmentAndThusLeaveLimits(employee, dateFromUser);
                    }
                }
                else
                {
                    Employee.AdjustDateOfRecruitmentAndThusLeaveLimits(employee, dateFromUser);
                }
            }

            Employee.SeeAndChangeLeaveLimits(employee);

            Employee.ChangeAccruedLeaveLimitPolicy(employee);
        }
//leave-related methods
        private bool IsLeaveLimitPolicySatisfied(Employee employee)
        {
            for (int i = employee.DayOfJoining.Year; i <= DateTime.Now.Year; i++)
            {
                if (CountLeaveAvailable(employee, i) < 0)
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsOnDemandLimitSatisfied(Employee employee, int newOnDemandLeaveLength)
        {
            int onDemandTaken = allLeavesInStorage.GetSumOnDemand(employee.Id);
            if (employee.OnDemandPerYear >= onDemandTaken + newOnDemandLeaveLength)
            {
                return true;
            }
            else
            {
                return false;
            }
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
        } //name may be misleading... "CurrentOnly" refers to for how many years not-taken past leave may be taken. In this method - only in current year.
        private int ExcessLeaveFromPastYearOneMore(Employee employee, int k)
        {
            LeaveLimit leaveLimitInYearK = employee.LeaveLimits.First(l => l.Year == k);
            int sumOfLeavesInYearK = allLeavesInStorage.CountSumOfPastYearLeaveDays(employee.Id, k);

            if (employee.DayOfJoining.Year != k)
            {
                return Math.Min(leaveLimitInYearK.Limit, (leaveLimitInYearK.Limit - sumOfLeavesInYearK + ExcessLeaveFromPastYearOneMore(employee, k - 1)));
            }
            else
            {
                return leaveLimitInYearK.Limit - sumOfLeavesInYearK;
            }
        }
        private int ExcessLeaveFromPastYearTwoMore(Employee employee, int k)
        {
            LeaveLimit leaveLimitInYearK = employee.LeaveLimits.First(l => l.Year == k);
            int sumOfLeavesInYearK = allLeavesInStorage.CountSumOfPastYearLeaveDays(employee.Id, k);

            if (employee.DayOfJoining.Year == k)
            {
                return (leaveLimitInYearK.Limit - sumOfLeavesInYearK);
            }
            else if (employee.DayOfJoining.Year == k - 1)
            {
                return (leaveLimitInYearK.Limit - sumOfLeavesInYearK + ExcessLeaveFromPastYearTwoMore(employee, k - 1));
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
            int sumOfLeavesInYearK = allLeavesInStorage.CountSumOfPastYearLeaveDays(employee.Id, k);

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
        private int CountLeaveAvailable(Employee employee, int year)
        {
            int result = 0;
            LeaveLimit leaveLimit = employee.LeaveLimits.First(l => l.Year == year);
            result = leaveLimit.Limit - allLeavesInStorage.GetSumOfDaysOnLeaveTakenByEmployeeInYear(employee.Id, year);
            Console.Write($"In {year} year there was {leaveLimit.Limit} limit and {allLeavesInStorage.GetSumOfDaysOnLeaveTakenByEmployeeInYear(employee.Id, year)} taken.");
            if (employee.DayOfJoining.Year < year)
            {
                result += CountExcessLeaveFromPast(employee, year - 1);
            }
            return result;
        }
        private void ShowLeaveAvailableForAllPastYears(Employee employee) //rather for test purpose
        {
            int startingYear = employee.DayOfJoining.Year;

            for (int i = startingYear; i <= DateTime.Now.Year; i++)
            {
                int k = CountLeaveAvailable(employee, i);
                Console.WriteLine($" Accrued leave available in {i}: {k}");
            }
        }
        public void AddLeave(int employeeId)
        {
            if(!EmployeeExists(employeeId))
            {
                return;
            };

            Employee employee = Employees.FirstOrDefault(e => e.Id == employeeId);

            int lastAddedLeaveId = allLeavesInStorage.Leaves.Count == 0 ? 0 : allLeavesInStorage.Leaves.LastOrDefault().Id;
            Leave leave = new(employeeId, lastAddedLeaveId + 1);

            Console.WriteLine("Default leave dates are set to: from {0}, to {1}. Do you want to keep them? Press enter if yes. Put n and enter if you want to set the dates manually", leave.DateFrom.ToString("yyyy-MM-dd"), leave.DateTo.ToString("yyyy-MM-dd"));
            if (Console.ReadLine() == "n")
            {
                Console.WriteLine("Put date - beginning of leave");
                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime userDateFrom))
                {
                    Console.WriteLine("The day of the week is: " + userDateFrom.DayOfWeek);
                    leave.DateFrom = userDateFrom;
                    if (!IsLeaveAfterDateOfRecruitment(employee, leave))
                    {
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("You entered an incorrect value.");
                }

                Console.WriteLine("Put date - end of leave");
                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime userDateTo))
                {
                    Console.WriteLine("The day of the week is: " + userDateTo.DayOfWeek);
                    leave.DateTo = userDateTo;
                    if (leave.DateFrom > leave.DateTo)
                    {
                        Console.WriteLine("Leave end cannot be older than leave start.");
                        return;
                    }

                    if (!Leave.IsLeaveInOneYear(leave))
                    {
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("You entered an incorrect value.");
                    return;
                }
            }

            if(!IsLeaveAfterDateOfRecruitment(employee, leave) || !Leave.IsLeaveInOneYear(leave))
            {
                return;
            }

            int ableOnDemand = 0;
            _ = IsOnDemandLimitSatisfied(employee, Leave.CountLeaveLength(leave)) ? ableOnDemand = 1 : 0;

            allLeavesInStorage.AddLeave(leave, ableOnDemand);
            ShowLeaveAvailableForAllPastYears(employee); //TEST PURPOSE

            if (!IsLeaveLimitPolicySatisfied(employee))
            {
                RemoveLeave(leave.Id);
                Console.WriteLine("Leave is not added. Leave limit policy is violated.");
            }
        }
        public void DisplayAllLeaves()
        {
            allLeavesInStorage.DisplayAllLeaves();
        }
        public void DisplayAllLeavesOnDemand()
        {
            allLeavesInStorage.DisplayAllLeavesOnDemand();
        }
        public void DisplayAllLeavesForEmployee(int employeeId)
        {
            allLeavesInStorage.DisplayAllLeavesForEmployee(employeeId);
        }
        public void DisplayAllLeavesForEmployeeOnDemand(int employeeId)
        {
            allLeavesInStorage.DisplayAllLeavesForEmployeeOnDemand(employeeId);
        }
        public void RemoveLeave(int intOfLeaveToRemove)
        {
            allLeavesInStorage.RemoveLeave(intOfLeaveToRemove);
        }
        public void EditLeave(int intOfLeaveToEdit)
        {
            var leaveToEdit = allLeavesInStorage.Leaves.FirstOrDefault(l => l.Id == intOfLeaveToEdit);

            if (leaveToEdit == null)
            {
                Console.WriteLine("Leave not found");
            }
            else
            {
                Employee employee = Employees.First(e => e.Id == leaveToEdit.EmployeeId);
                Leave leaveAuxiliary = new(leaveToEdit.EmployeeId, leaveToEdit.Id)
                {
                    DateFrom = leaveToEdit.DateFrom,
                    DateTo = leaveToEdit.DateTo,
                    IsOnDemand = leaveToEdit.IsOnDemand
                };

                Console.WriteLine("Put date of beginning of leave (or put any letter to skip)");

                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime userDateTimeFrom))
                {
                    Console.WriteLine("The day of the week is: " + userDateTimeFrom.DayOfWeek);
                    leaveAuxiliary.DateFrom = userDateTimeFrom;
                    if (!IsLeaveAfterDateOfRecruitment(employee, leaveAuxiliary))
                    {
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Date 'from' is not changed.");
                }

                Console.WriteLine("Put date of end of leave (or put any letter to skip)");
                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime userDateTimeTo))
                {
                    Console.WriteLine("The day of the week is: " + userDateTimeTo.DayOfWeek);
                    leaveAuxiliary.DateTo = userDateTimeTo;
                }
                else
                {
                    Console.WriteLine("Date 'to' is not changed.");
                }
                
                if (!Leave.IsLeaveInOneYear(leaveAuxiliary))
                {
                    return;
                }

                allLeavesInStorage.RemoveLeave(intOfLeaveToEdit);
                int ableOnDemand = 0;
                _ = IsOnDemandLimitSatisfied(employee, Leave.CountLeaveLength(leaveAuxiliary)) ? ableOnDemand = 1 : 0;

                if (leaveToEdit.IsOnDemand && ableOnDemand == 0)
                {
                    Console.WriteLine("The edited leave is On Demand. Yet after change it will not be possible due to exceeding On Demand leave limit per year. Do you want to proceeed and keep the leave as NOT On Demand? (put y if yes)");
                    if (Console.ReadLine() == "y")
                    {
                        leaveAuxiliary.IsOnDemand = false;
                        allLeavesInStorage.AddLeave(leaveAuxiliary, ableOnDemand);
                        ShowLeaveAvailableForAllPastYears(employee); //TEST PURPOSE

                        if (!IsLeaveLimitPolicySatisfied(employee))
                        {
                            allLeavesInStorage.RemoveLeave(intOfLeaveToEdit);
                            allLeavesInStorage.AddLeave(leaveToEdit, 0);
                            Console.WriteLine("Leave cannot be changed. Leave limit policy is violated.");
                        }
                    }
                    else
                    {
                        allLeavesInStorage.AddLeave(leaveToEdit, 0);
                        Console.WriteLine("Leave is not changed.");
                    }
                }
                else
                {
                    allLeavesInStorage.AddLeave(leaveAuxiliary, ableOnDemand);
                    ShowLeaveAvailableForAllPastYears(employee); //TEST PURPOSE

                    if (!IsLeaveLimitPolicySatisfied(employee))
                    {
                        allLeavesInStorage.RemoveLeave(intOfLeaveToEdit);
                        allLeavesInStorage.AddLeave(leaveToEdit, 0);
                        Console.WriteLine("Leave cannot be changed. Leave limit policy is violated.");
                    }
                }
            }
        }
    }
}
