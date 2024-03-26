namespace Inewi_Console.Entities
{
    public class EmployeeSettings(Employee employee)
    {
        public int Id { get; set; }
        public int LeavesPerYear { get; set; }
        public int OnDemandPerYear { get; set; }
        public int YearOfJoining { get; set; } = DateTime.Now.Year;
        public int LeavesInFirstYear { get; set; }
        public int OnDemandInFirstYear { get; set; }
        public Employee Employee { get; set; } = employee;
        public int EmployeeId { get; set; }
    }
}
