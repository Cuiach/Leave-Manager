namespace Leave_Manager.Leave_Manager.Core.Entities
{
    public class LeaveLimit
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Limit { get; set; }
        public Employee Employee { get; set; }
        public LeaveLimit(int year, int limit)
        {
            Year = year;
            Limit = limit;
        }
        public LeaveLimit()
        {
        }
    }
}