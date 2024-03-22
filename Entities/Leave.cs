namespace Inewi_Console.Entities
{
    public class Leave
    {
        public required int Id {  get; set; }
        public DateTime DateFrom {  get; set; } = DateTime.Now.AddDays(-7);
        public DateTime DateTo { get; set; } = DateTime.Now;
        public bool IsOnDemand { get; set; } = false;
    }
}
