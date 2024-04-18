using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Inewi_Console.Entities
{

    public class Employee(string fName, string lName, int numberAsId)
    {
        public int Id { get; set; } = numberAsId;
        public string FirstName { get; set; } = fName;
        public string LastName { get; set; } = lName;
        public int? WorkingYears { get; set; }
        public DateTime DayOfJoining { get; set; }
        public List<LeaveLimit> LeaveLimits { get; set; } = [];
        public enum YearsToTakeLeave
        {
            CurrentOnly,
            OneMore,
            TwoMore,
            NoLimit
        }
        public YearsToTakeLeave HowManyYearsToTakePastLeave = YearsToTakeLeave.OneMore;
        public int LeavesPerYear { get; set; } // currently rather auxiliary (leave limits are more important) - admin sets this as general number for the employee; it can be changed but only manually; usually 26
        public int OnDemandPerYear { get; set; }
    }
}
