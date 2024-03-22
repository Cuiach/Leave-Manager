namespace Inewi_Console.Entities
{
    public class Employee(string FName, string LName, int NumberAsId)
    {
        public int Id { get; set; } = NumberAsId;
        public string FirstName { get; set; } = FName;
        public string LastName { get; set; } = LName;
        public int? WorkingYears { get; set; }
        public EmployeeSettings? Setting { get; set; }

        //public void SetId() => Id = (Employees == null ? 1 : Employees.LastOrDefault().Id + 1);
    }
}
