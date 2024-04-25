namespace Inewi_Console.Entities
{
    public class Employee(string firstName, string lastName, int id)
    {
        public int Id { get; set; } = id;
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
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
        public int LeavesPerYear { get; set; }
        public int OnDemandPerYear { get; set; }

        internal void AdjustDateOfRecruitmentAndThusLeaveLimits(string newDateOfRecruitmentFromUser)
        {
            int oldYearOfRecruitment = this.DayOfJoining.Year;
            DateTime newDateOfRecruitment = DateTime.ParseExact(newDateOfRecruitmentFromUser, "yyyy-MM-dd", null);
            this.DayOfJoining = newDateOfRecruitment;
            Console.WriteLine($"Date of recruitment is set to: {newDateOfRecruitmentFromUser}");
            for (int i = oldYearOfRecruitment; i < newDateOfRecruitment.Year; i++)
            {
                LeaveLimit limitToRemove = this.LeaveLimits.FirstOrDefault(l => l.Year == i);
                this.LeaveLimits.Remove(limitToRemove);
            }
        }

        internal bool LeaveLimitForCurrentYearExists()
        {
            return this.LeaveLimits.FirstOrDefault(l => l.Year == DateTime.Now.Year) != null;
        }
    }
}