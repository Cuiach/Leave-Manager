using Inewi_Console.Entities;
using NuGet.Frameworks;
using static Inewi_Console.Entities.Employee;

namespace Inewi_Console.Tests
{
    public class EmployeeTests
    {
        [Fact]
        public void CreateEmployee_TwoEmployees_ReturnsCorrectEmployeeProperties()
        {
            //arrange
            ListOfEmployees newList = new();

            Employee employee1 = new("aaa", "AAA", 1)
            {
                DayOfJoining = new DateTime(2022, 03, 15),
                HowManyYearsToTakePastLeave = YearsToTakeLeave.OneMore,
                LeavesPerYear = 26,
                OnDemandPerYear = 4,
                LeaveLimits = [new LeaveLimit(2022, 20)]
            };
            newList.Employees.Add(employee1);

            Employee employee2 = new("bbb", "BBB", 2)
            {
                DayOfJoining = new DateTime(2021, 01, 01),
                HowManyYearsToTakePastLeave = YearsToTakeLeave.TwoMore,
                LeavesPerYear = 26,
                OnDemandPerYear = 4,
                LeaveLimits = [new LeaveLimit(2021, 26)]
            };
            newList.Employees.Add(employee2);

            foreach (Employee employee in newList.Employees)
            {
                for (int i = employee.DayOfJoining.Year + 1; i <= DateTime.Now.Year; i++)
                {
                    employee.LeaveLimits.Add(new LeaveLimit(i, employee.LeavesPerYear));
                }
            }

            //act
            bool leaveLimitsContainsValueFirstYear = employee1.LeaveLimits.Any(l => l.Year == employee1.DayOfJoining.Year);
            bool leaveLimitsContainsValueThisYear = employee1.LeaveLimits.Any(l => l.Year == DateTime.Now.Year);
            bool leaveLimitsContainsValueFirstYearEmployee2 = employee2.LeaveLimits.Any(l => l.Year == employee1.DayOfJoining.Year);
            bool leaveLimitsContainsValueThisYearEmployee2 = employee2.LeaveLimits.Any(l => l.Year == DateTime.Now.Year);

            //assert
            Assert.NotNull(employee1);
            Assert.Equal(26, employee1.LeavesPerYear);
            Assert.Equal(4, employee1.OnDemandPerYear);
            Assert.True(employee1.DayOfJoining.GetType() == typeof(DateTime));
            Assert.True(employee1.DayOfJoining > new DateTime(2000));
            Assert.True(employee1.DayOfJoining <= DateTime.Now);
            Assert.True(leaveLimitsContainsValueFirstYear);
            Assert.True(leaveLimitsContainsValueThisYear);

            Assert.NotNull(employee2);
            Assert.Equal(26, employee2.LeavesPerYear);
            Assert.Equal(4, employee2.OnDemandPerYear);
            Assert.True(employee2.DayOfJoining.GetType() == typeof(DateTime));
            Assert.True(employee2.DayOfJoining > new DateTime(2000));
            Assert.True(employee2.DayOfJoining <= DateTime.Now);
            Assert.True(leaveLimitsContainsValueFirstYearEmployee2);
            Assert.True(leaveLimitsContainsValueThisYearEmployee2);
        }
        [Fact]
        public void RemoveEmployee_ValidEmployee_RemovesEmployeeFromList()
        {
            // Arrange
            ListOfEmployees newList = new ListOfEmployees();

            Employee employee1 = new Employee("aaa", "AAA", 1)
            {
                DayOfJoining = new DateTime(2022, 03, 15),
                HowManyYearsToTakePastLeave = YearsToTakeLeave.OneMore,
                LeavesPerYear = 26,
                OnDemandPerYear = 4,
                LeaveLimits = { new LeaveLimit(2022, 20) }
            };
            newList.Employees.Add(employee1);

            Employee employee2 = new Employee("bbb", "BBB", 2)
            {
                DayOfJoining = new DateTime(2021, 01, 01),
                HowManyYearsToTakePastLeave = YearsToTakeLeave.TwoMore,
                LeavesPerYear = 26,
                OnDemandPerYear = 4,
                LeaveLimits = { new LeaveLimit(2021, 26) }
            };
            newList.Employees.Add(employee2);

            // Act
            newList.RemoveEmployee(employee1.Id);

            // Assert
            Assert.DoesNotContain(employee1, newList.Employees);
            Assert.Contains(employee2, newList.Employees);
        }
    }
}