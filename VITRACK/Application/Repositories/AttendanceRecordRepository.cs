using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VITRACK.Application.Interfaces;
using VITRACK.Infrastructure.Data;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Application.Repositories;

public sealed class AttendanceRecordRepository : IAttendanceRecordRepository
{
    private readonly AppDbContext _db;

    public AttendanceRecordRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AttendanceRecord> CreateAsync(AttendanceRecord attendanceRecord)
    {
        var entry = await _db.AttendanceRecords.AddAsync(attendanceRecord);
        await _db.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _db.AttendanceRecords.FindAsync(id);
        if (existing is null) return;
        _db.AttendanceRecords.Remove(existing);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(AttendanceRecord attendanceRecord)
    {
        _db.AttendanceRecords.Update(attendanceRecord);
        await _db.SaveChangesAsync();
    }
}
