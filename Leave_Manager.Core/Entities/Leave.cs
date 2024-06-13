using Leave_Manager.Leave_Manager.Core.Services;

namespace Leave_Manager.Leave_Manager.Core.Entities
{
    public class Leave
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool IsOnDemand { get; set; }

        public Leave()
        { }

        public Leave(int employeeId, bool isManuallyCreated)
        {
            EmployeeId = employeeId;

            if (isManuallyCreated)
            {
                DateFrom = DateTime.Today.Date.AddDays(-6);
                DateTo = DateTime.Today.Date;
                IsOnDemand = false;
            }
            else
            { }
        }

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
            { }
        }

        internal int GetLeaveLength()
        {
            WorkDaysCalculator workDaysCalculator = new();
            return workDaysCalculator.CountWorkDaysBetweenDates(DateFrom, DateTo);
        }

        internal int HowManyCalendarYearsLeaveSpans()
        {
            int yearFrom = DateFrom.Year;
            int yearTo = DateTo.Year;
            return yearFrom - yearTo + 1;
        }

        internal int GetWorkDaysOfLeave()
        {
            WorkDaysCalculator calculator = new();
            return calculator.CountWorkDaysBetweenDates(DateFrom, DateTo);
        }
    }
}