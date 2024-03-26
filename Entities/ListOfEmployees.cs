namespace Inewi_Console.Entities
{
    public class ListOfEmployees
    {
        public List<Employee> Employees { get; set; } = [];
        public List<EmployeeSettings> EmployeesSettings { get; set; } = [];

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

                Employees.Add(newContact);

                var newSettings = new EmployeeSettings(newContact)
                {
                    EmployeeId = idNewEmployee
                };

                Console.WriteLine("On Demand for the employee in THIS year - is it 4? If yes press enter; if not put correct number and enter");
                var onDemandDaysFirstYear = StaticMethods.GetChoice();
                newSettings.OnDemandInFirstYear = onDemandDaysFirstYear == "" ? 4 : StaticMethods.StringToIntExceptZero(onDemandDaysFirstYear);

                Console.WriteLine("Leave days THIS year for the employee - is it 26? If yes press enter; if not put correct number and enter");
                var leaveDaysFirstYear = StaticMethods.GetChoice();
                newSettings.LeavesInFirstYear = leaveDaysFirstYear == "" ? 26 : StaticMethods.StringToIntExceptZero(leaveDaysFirstYear);

                Console.WriteLine("On Demand per standard year for the employee - is it 4? If yes press enter; if not put correct number and enter");
                var onDemandDays = StaticMethods.GetChoice();
                newSettings.OnDemandPerYear = onDemandDays == "" ? 4 : StaticMethods.StringToIntExceptZero(onDemandDays);

                Console.WriteLine("Leave days per standard year for the employee - is it 26? If yes press enter; if not put correct number and enter");
                var leaveDays = StaticMethods.GetChoice();
                newSettings.LeavesPerYear = onDemandDays == "" ? 26 : StaticMethods.StringToIntExceptZero(leaveDays);

                EmployeesSettings.Add(newSettings);
            }
        }

        private void DisplayEmployeeDetails(Employee employee)
        {
            EmployeeSettings? settings = EmployeesSettings.FirstOrDefault(e => e.EmployeeId == employee.Id);
            Console.WriteLine($"Employee: {employee.FirstName}, {employee.LastName}, {employee.Id}, leave days per year: {settings?.LeavesPerYear}, on demand per year: {settings?.OnDemandPerYear}, year of joining: {settings.YearOfJoining}, leave days in first year: {settings.LeavesInFirstYear}, on demand days in first year: {settings.OnDemandInFirstYear}");
        }
        private void DisplayAllEmployeesDetails(List<Employee> employees)
        {
            foreach (var employee in employees)
            {
                DisplayEmployeeDetails(employee);
            }
        }
        public void DisplayEmployee(int number)
        {
            var employee = Employees.FirstOrDefault(c => c.Id == number);
            if (employee == null)
            {
                Console.WriteLine("Employee not found");
            }
            else
            {
                DisplayEmployeeDetails(employee);
            }
        }
        public void DisplayAllEmployees()
        {
            DisplayAllEmployeesDetails(Employees);
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
            } else
            {
                Console.WriteLine("No employees were found");
            }
        }
        public void RemoveEmployee(int id)
        {
            var employeeToRemove = Employees.FirstOrDefault(c => c.Id == id);
            if (employeeToRemove == null)
            {
                Console.WriteLine("Employee not found");
            }
            else
            {
                Employees.Remove(employeeToRemove);
            }
        }

        public void EditSettings(int intOfEmployeeToEdit)
        {
            var settings = EmployeesSettings.FirstOrDefault(s => s.EmployeeId == intOfEmployeeToEdit);

            Console.WriteLine("On Demand for the employee in THIS year - is it 4? If yes press enter; if not put correct number and enter");
            var onDemandDaysFirstYear = StaticMethods.GetChoice();
            settings.OnDemandInFirstYear = onDemandDaysFirstYear == "" ? 4 : StaticMethods.StringToIntExceptZero(onDemandDaysFirstYear);

            Console.WriteLine("Leave days THIS year for the employee - is it 26? If yes press enter; if not put correct number and enter");
            var leaveDaysFirstYear = StaticMethods.GetChoice();
            settings.LeavesInFirstYear = leaveDaysFirstYear == "" ? 26 : StaticMethods.StringToIntExceptZero(leaveDaysFirstYear);

            Console.WriteLine("On Demand per year for the employee - is it 4? If yes press enter; if not put correct number and enter");
            var onDemandDays = StaticMethods.GetChoice();
            settings.OnDemandPerYear = onDemandDays == "" ? 4 : StaticMethods.StringToIntExceptZero(onDemandDays);

            Console.WriteLine("Leave days per year for the employee - is it 26? If yes press enter; if not put correct number and enter");
            var leaveDays = StaticMethods.GetChoice();
            settings.LeavesPerYear = onDemandDays == "" ? 26 : StaticMethods.StringToIntExceptZero(leaveDays);

        }
    }
}
