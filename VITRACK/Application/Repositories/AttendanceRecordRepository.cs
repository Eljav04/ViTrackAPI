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
            .Include(ar => ar.Employee)
                .ThenInclude(e => e!.Department)
            .Select(at => new AttendanceBasicInfo
            {
                Id = at.Id,
                Employee = at.Employee != null ? new EmployeeBasicInfo
                {
                    Id = at.Employee.Id,
                    Firstname = at.Employee.Firstname,
                    Lastname = at.Employee.Surname,
                    DepartmentName = at.Employee.Department != null ? at.Employee.Department.Name : null,
                    WorkStartTime = at.PlannedStartTime,
                    WorkEndTime = at.PlannedEndTime
                } : null,
                Date = at.Date,
                ArrivalTime = at.ArrivalTime,
                LeaveTime = at.LeaveTime,
                QrApprovedArrival = at.QrApprovedArrival,
                QrApprovedLeave = at.QrApprovedLeave,
                LocationApprovedArrival = at.ArrivalLongitude != null,
                ArrivalLocation = at.ArrivalLongitude != null && at.ArrivalLatitude != null ? new LocationBasicInfo
                {
                    Longitude = at.ArrivalLongitude,
                    Latitude = at.ArrivalLatitude
                } : null,
                LocationApprovedLeave = at.LeaveLongitude != null,
                LeaveLocation = at.LeaveLongitude != null && at.LeaveLatitude != null ? new LocationBasicInfo
                {
                    Longitude = at.LeaveLongitude,
                    Latitude = at.LeaveLatitude
                } : null,
                ArrivalImage = at.ArrivalImgUrl,
                LeaveImage = at.LeaveImgUrl,
                LateReason = at.LateReason,
                EarlyLeaveReason = at.EarlyLeaveReason,
                IsLate = at.IsLate,
                IsEarlyLeave = at.IsEarlyLeave,
                IsAbsent = at.IsAbsent,
                IsRest = at.IsRest,
                AttendanceDurationMinutes = at.AttendanceDurationMinutes,
                OvertimeMinutes = at.OvertimeMinutes,
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
            .Include(ar => ar.Employee)
                .ThenInclude(e => e!.Department)
            .Select(at => new AttendanceBasicInfo
            {
                Id = at.Id,
                Employee = at.Employee != null ? new EmployeeBasicInfo
                {
                    Id = at.Employee.Id,
                    Firstname = at.Employee.Firstname,
                    Lastname = at.Employee.Surname,
                    DepartmentName = at.Employee.Department != null ? at.Employee.Department.Name : null,
                    WorkStartTime = at.PlannedStartTime,
                    WorkEndTime = at.PlannedEndTime
                } : null,
                Date = at.Date,
                ArrivalTime = at.ArrivalTime,
                LeaveTime = at.LeaveTime,
                QrApprovedArrival = at.QrApprovedArrival,
                QrApprovedLeave = at.QrApprovedLeave,
                LocationApprovedArrival = at.ArrivalLongitude != null,
                ArrivalLocation = at.ArrivalLongitude != null && at.ArrivalLatitude != null ? new LocationBasicInfo
                {
                    Longitude = at.ArrivalLongitude,
                    Latitude = at.ArrivalLatitude
                } : null,
                LocationApprovedLeave = at.LeaveLongitude != null,
                LeaveLocation = at.LeaveLongitude != null && at.LeaveLatitude != null ? new LocationBasicInfo
                {
                    Longitude = at.LeaveLongitude,
                    Latitude = at.LeaveLatitude
                } : null,
                ArrivalImage = at.ArrivalImgUrl,
                LeaveImage = at.LeaveImgUrl,
                LateReason = at.LateReason,
                EarlyLeaveReason = at.EarlyLeaveReason,
                IsLate = at.IsLate,
                IsEarlyLeave = at.IsEarlyLeave,
                IsAbsent = at.IsAbsent,
                IsRest = at.IsRest,
                AttendanceDurationMinutes = at.AttendanceDurationMinutes,
                OvertimeMinutes = at.OvertimeMinutes,
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
