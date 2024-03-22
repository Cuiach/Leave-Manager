namespace Inewi_Console.Entities
{
    public class EmployeeSettings
    {
        public int Id { get; set; }
        public int LeavesPerYear { get; set; }
        public int OnDemandPerYear { get; set; }
        public required Employee Employee { get; set; }
        public int EmployeeId { get; set; }
    }
}
