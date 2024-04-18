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
        private bool IsEmployeeExists(int employeeId)
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
                int CalculateLeaveDaysTakenThisYear()
                {
                    return allLeavesInStorage.GetSumOfDaysOnLeaveTakenByEmployeeInYear(employee.Id, DateTime.Now.Year);
                }

                int CalculateOnDemandTaken()
                {
                    return allLeavesInStorage.GetSumOnDemand(employee.Id);
                }

                DisplayEmployeeDetails(employee, CalculateLeaveDaysTakenThisYear(), CalculateOnDemandTaken());
            }
        }
        private static void AdjustDateOfRecruitmentAndThusLeaveLimits(Employee employee, string newDateOfRecruitmentFromUser)
        {
            int oldYearOfRecruitment = employee.DayOfJoining.Year;
            DateTime newDateOfRecruitment = DateTime.ParseExact(newDateOfRecruitmentFromUser, "yyyy-MM-dd", null);
            employee.DayOfJoining = newDateOfRecruitment;
            Console.WriteLine($"Date of recruitment is set to: {newDateOfRecruitmentFromUser}");
                        for (int i = oldYearOfRecruitment; i<newDateOfRecruitment.Year; i++)
                        {
                            LeaveLimit limitToRemove = employee.LeaveLimits.FirstOrDefault(l => l.Year == i);
                            employee.LeaveLimits.Remove(limitToRemove);
                        }
        }
        private static void SeeAndChangeLeaveLimits(Employee employee)
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
                choice = StaticMethods.GetChoice();

                if (choice == "y")
                {
                    Console.WriteLine("Put limit:");
                    string? newLimit = StaticMethods.GetChoice();
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
        private static void ChangeAccruedLeaveLimitPolicy(Employee employee)
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
            choice = StaticMethods.GetChoice();
            
            if (choice == "y")
            {
                Console.WriteLine("Choose accrued leave cap: \n0 = only current year \n1 = current year and last year \n2 = current and two last years \n3 = no cap");
                string? newLeaveYearsLimit = StaticMethods.GetChoice();
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
        public void AddEmployee()
        {
            Console.WriteLine("Insert first name");
            var firstName = Console.ReadLine();
            Console.WriteLine("Insert last name");
            var lastName = Console.ReadLine();
            
            if (firstName != null && lastName != null)
            {
                int idNewEmployee = Employees.Count == 0 ? 1 : Employees.LastOrDefault().Id + 1;

                var newContact = new Employee(firstName, lastName, idNewEmployee);

                Console.WriteLine("On Demand for the employee per year - is it 4? If yes press enter; if not put correct number and enter");
                var onDemandPerYear = StaticMethods.GetChoice();
                newContact.OnDemandPerYear = onDemandPerYear == "" ? 4 : StaticMethods.StringToIntExceptZero(onDemandPerYear);

                Console.WriteLine("Leave days per year for the employee - is it 26? If yes press enter; if not put correct number and enter");
                var leavesPerYearString = StaticMethods.GetChoice();
                int leavesPerYear;
                _ = leavesPerYearString == "" ? leavesPerYear = 26 : leavesPerYear = StaticMethods.StringToIntExceptZero(leavesPerYearString);
                newContact.LeavesPerYear = leavesPerYearString == "" ? 26 : leavesPerYear;

                int currentYear = StaticMethods.StringToIntExceptZero(DateTime.Now.Year.ToString());
                LeaveLimit leavelimit = new(currentYear, leavesPerYear);
                newContact.LeaveLimits.Add(leavelimit);
                newContact.DayOfJoining = DateTime.Now;

                Employees.Add(newContact);
                Console.WriteLine("Day of joing of employee is set to today. If you want to change it go to edit settings");
            }
        }
        public void DisplayAllEmployees()
        {
            DisplayAllEmployeesDetails(Employees);
        }
        public void RemoveEmployee(int employeeId)
        {
            if (!IsEmployeeExists(employeeId))
            {
                return;
            }
            else
            {
                var employeeToRemove = Employees.FirstOrDefault(c => c.Id == employeeId);
                Employees.Remove(employeeToRemove);
            }
        }
        public void DisplayMatchingEmployees(string searchPhrase)
        {
            var matchingEmployees = new List<Employee>();

            foreach (var employee in Employees)
            {
                if (employee.FirstName == searchPhrase || employee.LastName == searchPhrase)
                    matchingEmployees.Add(employee);
            }

            if (matchingEmployees != null && matchingEmployees.Count != 0)
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
            if (!IsEmployeeExists(employeeIdToEdit))
            {
                return;
            };

            string? choice;
            var employee = Employees.FirstOrDefault(e => e.Id == employeeIdToEdit);
            int oldYearOfRecruitment = employee.DayOfJoining.Year;

            Console.WriteLine($"On Demand for the employee per year - is it {employee.OnDemandPerYear}? If yes press enter; if not put correct number and enter");
            var onDemandPerYear = StaticMethods.GetChoice();
            employee.OnDemandPerYear = onDemandPerYear == "" ? employee.OnDemandPerYear : StaticMethods.StringToIntExceptZero(onDemandPerYear);

            Console.WriteLine($"Leave days per year for the employee - is it {employee.LeavesPerYear}? If yes press enter; if not put correct number and enter");
            var leavesPerYear = StaticMethods.GetChoice();
            employee.LeavesPerYear = leavesPerYear == "" ? employee.LeavesPerYear : StaticMethods.StringToIntExceptZero(leavesPerYear);
            LeaveLimit leaveLimitThisYear = employee.LeaveLimits.FirstOrDefault(l => l.Year == DateTime.Now.Year);
            if (leaveLimitThisYear == null)
            {
                leaveLimitThisYear = new(DateTime.Now.Year, employee.LeavesPerYear);
                employee.LeaveLimits.Add(leaveLimitThisYear);
            }
            else
            {
                leaveLimitThisYear.Limit = StaticMethods.StringToIntExceptZero(leavesPerYear);
            }

            Console.WriteLine("Do you want to change employee's date of recruitment? Press y if yes");
            choice = StaticMethods.GetChoice();
            if (choice == "y")
            {
                DateTime newDateOfRecruitment;
                string dateFromUser = "";
                while (!StaticMethods.IsValidDate(dateFromUser))
                {
                    Console.WriteLine("Enter a date of recruitment (yyyy-MM-dd): ");
                    dateFromUser = StaticMethods.GetChoice();
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
                        AdjustDateOfRecruitmentAndThusLeaveLimits(employee, dateFromUser);
                    }
                }
                else
                {
                    AdjustDateOfRecruitmentAndThusLeaveLimits(employee, dateFromUser);
                }
            }

            SeeAndChangeLeaveLimits(employee);

            ChangeAccruedLeaveLimitPolicy(employee);
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
        private static bool IsLeaveInOneYear(Leave leave)
        {
            if (leave.DateFrom.Year != leave.DateTo.Year)
            {
                Console.WriteLine("Leave must be within one calendar year. Try again with correct dates.");
                return false;
            }
            else
            {
                return true;
            }
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
        private int ExcessLeaveFromPastYearCurrentOnly(Employee employee, int k)
        {
            return 0;
        }
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
        public void AddLeave(int employeeId)
        {
            if(!IsEmployeeExists(employeeId))
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

                    if (!IsLeaveInOneYear(leave))
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

            if(!IsLeaveAfterDateOfRecruitment(employee, leave) || !IsLeaveInOneYear(leave))
            {
                return;
            }

            int ableOnDemand = 0;
            _ = IsOnDemandLimitSatisfied(employee, StaticMethods.CountLeaveLength(leave)) ? ableOnDemand = 1 : 0;

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
                
                if (!IsLeaveInOneYear(leaveAuxiliary))
                {
                    return;
                }

                allLeavesInStorage.RemoveLeave(intOfLeaveToEdit);
                int ableOnDemand = 0;
                _ = IsOnDemandLimitSatisfied(employee, StaticMethods.CountLeaveLength(leaveAuxiliary)) ? ableOnDemand = 1 : 0;

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
