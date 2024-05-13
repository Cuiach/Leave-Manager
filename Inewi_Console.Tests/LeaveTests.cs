using Inewi_Console.Entities;

namespace Inewi_Console.Tests
{
    public class LeaveTests
    {
        [Fact]
        public void SplitLeaveIntoConsecutiveBusinessDaysBits_Leave_ReturnsCorrectLeaveProperties()
        {
            //arrange
            HistoryOfLeaves historyOfLeaves = new();
            Leave leave = new(1, 1, true)
            {
                DateFrom = new DateTime(2024, 05, 01),
                DateTo = new DateTime(2024, 05, 10)
            };
            historyOfLeaves.Leaves.Add(leave);

            //act
            historyOfLeaves.SplitLeaveIntoConsecutiveBusinessDaysBits(leave);

            //assert
            Assert.NotNull(historyOfLeaves);
            Assert.Equal(2, historyOfLeaves.Leaves.Count);
            Assert.Equal(1, historyOfLeaves.Leaves[0].EmployeeId);
            Assert.Equal(1, historyOfLeaves.Leaves[0].Id);
            Assert.False(historyOfLeaves.Leaves[0].IsOnDemand);
            Assert.Equal(new DateTime(2024, 05, 02), historyOfLeaves.Leaves[0].DateFrom);
            Assert.Equal(new DateTime(2024, 05, 02), historyOfLeaves.Leaves[0].DateTo);
            Assert.Equal(1, historyOfLeaves.Leaves[1].EmployeeId);
            Assert.Equal(2, historyOfLeaves.Leaves[1].Id);
            Assert.False(historyOfLeaves.Leaves[1].IsOnDemand);
            Assert.Equal(new DateTime(2024, 05, 06), historyOfLeaves.Leaves[1].DateFrom);
            Assert.Equal(new DateTime(2024, 05, 10), historyOfLeaves.Leaves[1].DateTo);
        }

        [Fact]
        public void AddLeave_AddsLeaveToHistory()
        {
            //arrange
            HistoryOfLeaves historyOfLeaves = new();
            Leave leave = new(1, 1, true)
            {
                DateFrom = new DateTime(2024, 05, 01),
                DateTo = new DateTime(2024, 05, 10)
            };

            //act
            historyOfLeaves.AddLeave(leave, false);
            historyOfLeaves.SplitLeaveIntoConsecutiveBusinessDaysBits(leave);

            //assert
            Assert.NotNull(historyOfLeaves);
            Assert.Equal(2, historyOfLeaves.Leaves.Count);
        }

        [Fact]
        public void RemoveLeave_RemovesLeaveFromHistory()
        {
            //arrange
            HistoryOfLeaves historyOfLeaves = new();
            Leave leave = new(1, 1, true)
            {
                DateFrom = new DateTime(2024, 05, 01),
                DateTo = new DateTime(2024, 05, 15)
            };
            historyOfLeaves.AddLeave(leave, false);
            historyOfLeaves.SplitLeaveIntoConsecutiveBusinessDaysBits(leave);

            //act
            historyOfLeaves.RemoveLeave(1);
            historyOfLeaves.RemoveLeave(2);

            //assert
            Assert.NotNull(historyOfLeaves);
            Assert.Single(historyOfLeaves.Leaves);
            Assert.Equal(new DateTime(2024, 05, 13), historyOfLeaves.Leaves[0].DateFrom);
            Assert.Equal(new DateTime(2024, 05, 15), historyOfLeaves.Leaves[0].DateTo);
        }

        [Fact]
        public void CheckOnDemandLeave_LeaveIsShorterThan5WorkingDays()
        {
            //arrange
            HistoryOfLeaves historyOfLeaves = new();
            Leave leave = new(1, 1, true)
            {
                DateFrom = new DateTime(2024, 05, 01),
                DateTo = new DateTime(2024, 05, 04),
                IsOnDemand = true
            };

            //act
            historyOfLeaves.AddLeave(leave, false);
            historyOfLeaves.SplitLeaveIntoConsecutiveBusinessDaysBits(leave);

            //assert
            Assert.NotNull(historyOfLeaves);
            Assert.Single(historyOfLeaves.Leaves);
            Assert.True(historyOfLeaves.Leaves[0].IsOnDemand);
            Assert.Equal(new DateTime(2024, 05, 02), historyOfLeaves.Leaves[0].DateFrom);
        }
    }
}
