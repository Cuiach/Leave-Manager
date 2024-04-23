using System.ComponentModel.DataAnnotations;

namespace Inewi_Console.Entities
{
    public class Leave(int employeeId, int numberAsId)
    {
        public int Id { get; set; } = numberAsId;
        public int EmployeeId { get; set; } = employeeId;
        public DateTime DateFrom {  get; set; } = DateTime.Now.Date.AddDays(-6);
        public DateTime DateTo { get; set; } = DateTime.Now.Date;
        public bool IsOnDemand { get; set; } = false;
        internal static void DisplayLeaveDetails(Leave leave)
        {
            string onDemand = "ON DEMAND";
            string notOnDemand = "NOT On Demand";
            Console.WriteLine($"Leave details Id={leave.Id}, Employee Id={leave.EmployeeId}, leave from: {leave.DateFrom}, leave to: {leave.DateTo}, {(leave.IsOnDemand ? onDemand : notOnDemand)}");
        }
        public static int CountLeaveLength(Leave leave)
        {
            return (leave.DateTo - leave.DateFrom).Days + 1;
        }
    }
}
