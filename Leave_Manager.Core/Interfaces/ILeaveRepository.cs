using Leave_Manager.Leave_Manager.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leave_Manager.Leave_Manager.Core.Interfaces
{
    public interface ILeaveRepository
    {
        Task<List<Leave>> GetAllLeavesAsync();
        Task<Leave?> GetLeaveByIdAsync(int id);
        Task<int> AddLeaveAsync(Leave leave);
        Task UpdateLeaveAsync(Leave leave);
        Task DeleteLeaveAsync(int id);
    }
}
