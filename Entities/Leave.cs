using System.ComponentModel.DataAnnotations;

namespace Inewi_Console.Entities
{
    public class Leave(int employeeId, int numberAsId)
    {
        public int Id { get; set; } = numberAsId;
        public int EmployeeId { get; set; } = employeeId;
        public DateTime DateFrom {  get; set; } = DateTime.Now.AddDays(-7);
        public DateTime DateTo { get; set; } = DateTime.Now;
        public bool IsOnDemand { get; set; } = false;
    }
}
