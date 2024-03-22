using Inewi_Console.Entities;

Console.WriteLine("--- Leaves of employees management app ---");

Console.WriteLine("1 Add employee");
Console.WriteLine("2 Display all employees");
Console.WriteLine("3 Search employee");
Console.WriteLine("4 Remove employee");
Console.WriteLine(" To exit insert 'x'");

var userInput = Console.ReadLine();

var listOfEmployees = new ListOfEmployees();

while (true)
{
    switch (userInput)
    {
        case "1":

            Console.WriteLine("Insert first name");
            var firstName = Console.ReadLine();
            Console.WriteLine("Insert last name");
            var lastName = Console.ReadLine();
            if (firstName != null && lastName != null)
            {
                int idNewEmployee = listOfEmployees.Employees.Count == 0 ? 1 : listOfEmployees.Employees.LastOrDefault().Id + 1;
                
                var newContact = new Employee(firstName, lastName, idNewEmployee);

                listOfEmployees.AddEmployee(newContact);
            }

            break;
        case "2":
            listOfEmployees.DisplayAllEmployees();
            break;
        case "3":
            Console.WriteLine("Insert search phrase");
            var searchPhrase = (Console.ReadLine() ?? ""); //null checker
            listOfEmployees.DisplayMatchingEmployees(searchPhrase);
            break;
        case "4":
            Console.WriteLine("Insert id");
            var nameToRemove = (Console.ReadLine() ?? "0");
            int intToRemove;
            bool _ = int.TryParse(nameToRemove, out intToRemove);
            if (intToRemove != 0)
            {
                listOfEmployees.RemoveEmployee(intToRemove);
            }
            break;
        case "x":
            return;
        default:
            Console.WriteLine("Invalid operation");
            break;
    }

    Console.WriteLine("Select operation");
    userInput = Console.ReadLine();
}