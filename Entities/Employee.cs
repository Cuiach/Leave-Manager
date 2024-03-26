namespace Inewi_Console.Entities
{
    public class Employee(string fName, string lName, int numberAsId)
    {
        public int Id { get; set; } = numberAsId;
        public string FirstName { get; set; } = fName;
        public string LastName { get; set; } = lName;
        public int? WorkingYears { get; set; }
        public EmployeeSettings? Setting { get; set; }
    }
}
