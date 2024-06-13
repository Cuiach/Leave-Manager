namespace Leave_Manager.Leave_Manager.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
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
        public int LeavesPerYear { get; set; }
        public int OnDemandPerYear { get; set; }

        public Employee()
        {
        }

        public Employee(string firstName, string lastName, int id)
        {
            FirstName = firstName;
            LastName = lastName;
            Id = id;
        }

        public Employee(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        internal void AdjustDateOfRecruitmentAndThusLeaveLimits(string newDateOfRecruitmentFromUser)
        {
            int oldYearOfRecruitment = DayOfJoining.Year;
            DateTime newDateOfRecruitment = DateTime.ParseExact(newDateOfRecruitmentFromUser, "yyyy-MM-dd", null);
            DayOfJoining = newDateOfRecruitment;
            for (int i = oldYearOfRecruitment; i < newDateOfRecruitment.Year; i++)
            {
                LeaveLimit limitToRemove = LeaveLimits.FirstOrDefault(l => l.Year == i);
                LeaveLimits.Remove(limitToRemove);
            }
        }

        internal bool LeaveLimitForCurrentYearExists()
        {
            return LeaveLimits.FirstOrDefault(l => l.Year == DateTime.Now.Year) != null;
        }

        internal void PropagateLeaveLimitForCurrentYear(int leavesPerYear, bool isItNewEmployee)
        {
            int currentYear = DateTime.Today.Year;
            LeavesPerYear = leavesPerYear;

            if (isItNewEmployee)
            {
                LeaveLimit leavelimit = new(currentYear, LeavesPerYear);
                LeaveLimits.Add(leavelimit);
            }
            else
            {
                LeaveLimit leavelimit;
                leavelimit = LeaveLimits.FirstOrDefault(l => l.Year == currentYear);
                if (leavelimit != null)
                {
                    leavelimit.Limit = LeavesPerYear;
                }
                else
                {
                    leavelimit = new(currentYear, LeavesPerYear);
                    LeaveLimits.Add(leavelimit);
                }
            }
        }
    }
}