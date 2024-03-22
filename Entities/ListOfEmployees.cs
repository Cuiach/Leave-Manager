namespace Inewi_Console.Entities
{
    public class ListOfEmployees
    {
        public List<Employee> Employees { get; set; } = [];

        public void AddEmployee(Employee employee)
        {
            Employees.Add(employee);
        }
        private static void DisplayEmployeeDetails(Employee employee)
        {
            Console.WriteLine($"Employee: {employee.FirstName}, {employee.LastName}, {employee.Id}");
        }
        private static void DisplayEmployeesDetails(List<Employee> employees)
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
            DisplayEmployeesDetails(Employees);
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
                DisplayEmployeesDetails(matchingEmployees);
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
    }
}
