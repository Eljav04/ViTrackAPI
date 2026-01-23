using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VITRACK.Api.DTOs.AttendanceRecords;
using VITRACK.Application.Interfaces;
using VITRACK.Common.RequestFeatures;
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

    public async Task<PagedList<AttendanceBasicInfo>> GetAllAsync(AttendanceParametrs attendanceParametrs)
    {
        var items = await _db.AttendanceRecords
            .OrderByDescending(ar => ar.CreatedAt)
            .Skip((attendanceParametrs.PageNumber - 1) * attendanceParametrs.PageSize)
            .Take(attendanceParametrs.PageSize)
            .Select(at => new AttendanceBasicInfo
            {
                Id = at.Id,
                EmployeeId = at.EmployeeId,
                Date = at.Date,
                ArrivalTime = at.ArrivalTime,
                LeaveTime = at.LeaveTime,
                QrApprovedArrival = at.QrApprovedArrival,
                QrApprovedLeave = at.QrApprovedLeave,
                LocationApprovedArrival = at.ArrivalLongitude != null,
                LocationApprovedLeave = at.LeaveLongitude != null,
                HasArrivalImage = at.ArrivalImgUrl != null,
                HasLeaveImage = at.LeaveImgUrl != null,
                IsLate = at.IsLate,
                IsEarlyLeave = at.IsEarlyLeave,
                CreatedAt = at.CreatedAt,
                UpdatedAt = at.UpdatedAt
            })
            .ToListAsync();

        var count = await _db.AttendanceRecords.CountAsync();

        return PagedList<AttendanceBasicInfo>
            .ToPagedList(
                items,
                count,
                attendanceParametrs.PageNumber,
                attendanceParametrs.PageSize);
    }

    public async Task<PagedList<AttendanceBasicInfo>> GetByEmployeeIdAsync(string employeeId, AttendanceParametrs attendanceParametrs)
    {
        var items = await _db.AttendanceRecords
            .Where(ar => ar.EmployeeId == employeeId)
            .OrderByDescending(ar => ar.CreatedAt)
            .Skip((attendanceParametrs.PageNumber - 1) * attendanceParametrs.PageSize)
            .Take(attendanceParametrs.PageSize)
            .Select(at => new AttendanceBasicInfo
            {
                Id = at.Id,
                EmployeeId = at.EmployeeId,
                Date = at.Date,
                ArrivalTime = at.ArrivalTime,
                LeaveTime = at.LeaveTime,
                QrApprovedArrival = at.QrApprovedArrival,
                QrApprovedLeave = at.QrApprovedLeave,
                LocationApprovedArrival = at.ArrivalLongitude != null,
                LocationApprovedLeave = at.LeaveLongitude != null,
                HasArrivalImage = at.ArrivalImgUrl != null,
                HasLeaveImage = at.LeaveImgUrl != null,
                IsLate = at.IsLate,
                IsEarlyLeave = at.IsEarlyLeave,
                CreatedAt = at.CreatedAt,
                UpdatedAt = at.UpdatedAt
            })
            .ToListAsync();

        var count = await _db.AttendanceRecords
            .Where(ar => ar.EmployeeId == employeeId)
            .CountAsync();

        return PagedList<AttendanceBasicInfo>
            .ToPagedList(
                items,
                count,
                attendanceParametrs.PageNumber,
                attendanceParametrs.PageSize);
    }
    public async Task<AttendanceRecord?> GetByDateAsync(string employeeId, DateOnly date)
    {
        return await _db.AttendanceRecords
            .FirstOrDefaultAsync(ar => ar.EmployeeId == employeeId && ar.Date == date);
    }


    public async Task<AttendanceRecord?> GetByIdAsync(int id)
    {
        return await _db.AttendanceRecords.FirstOrDefaultAsync(ar => ar.Id == id);
    }

    public async Task UpdateAsync(AttendanceRecord attendanceRecord)
    {
        _db.AttendanceRecords.Update(attendanceRecord);
        await _db.SaveChangesAsync();
    }
}
