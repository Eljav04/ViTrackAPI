using System.Collections.Generic;
using System.Threading.Tasks;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Application.Interfaces;

public interface IWorkScheduleRepository
{
    Task<IEnumerable<WorkSchedule>> GetAllAsync();
    Task<WorkSchedule?> GetByIdAsync(int id);
    Task<WorkSchedule> CreateAsync(WorkSchedule workSchedule);
    Task UpdateAsync(WorkSchedule workSchedule);
    Task DeleteAsync(int id);
    Task<bool> AssignUserToWorkScheduleAsync(int workScheduleId, string userId);
    Task<bool> RemoveUserFromWorkScheduleAsync(int workScheduleId, string userId);
}
