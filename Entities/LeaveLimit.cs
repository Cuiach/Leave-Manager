namespace Leave_Manager_Console.Entities
{
    public class LeaveLimit
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Limit { get; set; }
        public Employee Employee { get; set; }
        public int EmployeeId { get; set; }
        public LeaveLimit(int year, int limit)
        {
            this.Year = year;
            this.Limit = limit;
        }
        public LeaveLimit()
        {
        }
    }
}