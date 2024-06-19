using Leave_Manager.Leave_Manager.Core.Entities;
using Leave_Manager.Leave_Manager.Core.Interfaces;
using Leave_Manager.Leave_Manager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leave_Manager.Leave_Manager.Infrastructure.Persistence
{
    internal class LeaveRepository : ILeaveRepository
    {
        private readonly LMDbContext _context;

        public LeaveRepository(LMDbContext context)
        {
            _context = context;
        }

        public async Task<List<Leave>> GetAllLeavesAsync()
        {
            return await _context.Leaves.ToListAsync();
        }

        public async Task<Leave?> GetLeaveByIdAsync(int id)
        {
            return await _context.Leaves.FindAsync(id);
        }

        public async Task<int> AddLeaveAsync(Leave leave)
        {
            await _context.Leaves.AddAsync(leave);
            await _context.SaveChangesAsync();
            return leave.Id;
        }

        public async Task UpdateLeaveAsync(Leave leave)
        {
            _context.Entry(leave).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLeaveAsync(int id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave != null)
            {
                _context.Leaves.Remove(leave);
                await _context.SaveChangesAsync();
            }
        }
    }
}
