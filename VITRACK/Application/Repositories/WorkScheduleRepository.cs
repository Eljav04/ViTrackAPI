using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VITRACK.Application.Interfaces;
using VITRACK.Infrastructure.Data;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Application.Repositories;

public sealed class WorkScheduleRepository : IWorkScheduleRepository
{
    private readonly AppDbContext _db;

    public WorkScheduleRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<WorkSchedule>> GetAllAsync()
    {
        return await _db.WorkSchedules.AsNoTracking().ToListAsync();
    }

    public async Task<WorkSchedule?> GetByIdAsync(int id)
    {
        return await _db.WorkSchedules.FindAsync(id);
    }

    public async Task<WorkSchedule> CreateAsync(WorkSchedule workSchedule)
    {
        var entry = await _db.WorkSchedules.AddAsync(workSchedule);
        await _db.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task UpdateAsync(WorkSchedule workSchedule)
    {
        _db.WorkSchedules.Update(workSchedule);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _db.WorkSchedules.FindAsync(id);
        if (existing is null) return;
        _db.WorkSchedules.Remove(existing);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> AssignUserToWorkScheduleAsync(int workScheduleId, string userId)
    {
        var ws = await _db.WorkSchedules.FindAsync(workScheduleId);
        if (ws is null) return false;

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return false;

        user.WorkScheduleId = workScheduleId;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveUserFromWorkScheduleAsync(int workScheduleId, string userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return false;

        if (user.WorkScheduleId != workScheduleId) return false;

        user.WorkScheduleId = null;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return true;
    }
}
