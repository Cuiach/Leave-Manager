namespace Inewi_Console.Entities
{
    public class Leave
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool IsOnDemand { get; set; }

        public Leave(int employeeId, int numberAsId, bool isManuallyCreated)
        {
            Id = numberAsId;
            EmployeeId = employeeId;

            if (isManuallyCreated)
            {
                DateFrom = DateTime.Today.Date.AddDays(-6);
                DateTo = DateTime.Today.Date;
                IsOnDemand = false;
            }
            else
            {}
        }

        internal static void DisplayLeaveDetails(Leave leave)
        {
            string onDemand = "ON DEMAND";
            string notOnDemand = "NOT On Demand";
            Console.WriteLine($"Leave details Id={leave.Id}, Employee Id={leave.EmployeeId}, leave from: {leave.DateFrom}, leave to: {leave.DateTo}, {(leave.IsOnDemand ? onDemand : notOnDemand)}");
        }
        
        internal int GetLeaveLength()
        {
            return (DateTo - DateFrom).Days + 1;
        }
 
        internal bool IsLeaveInOneYear()
        {
            if (DateFrom.Year != DateTo.Year)
            {
                Console.WriteLine("Leave must be within one calendar year. Try again with correct dates.");
                return false;
            }
            else
            {
                return true;
            }
        }

        internal int GetWorkDaysOfLeave()
        {
            WorkDaysCalculator calculator = new();
            return calculator.CountWorkDaysBetweenDates(DateFrom, DateTo);
        }
    }
}