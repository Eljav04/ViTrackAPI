using System.Collections.Generic;
using System.Threading.Tasks;
using VITRACK.Api.DTOs.AttendanceRecords;
using VITRACK.Common.RequestFeatures;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Application.Interfaces;

public interface IAttendanceRecordRepository
{
    Task<AttendanceRecord?> GetByDateAsync(string employeeId, DateOnly date);
    Task<AttendanceRecord?> GetByIdAsync(int id);
    Task<PagedList<AttendanceBasicInfo>> GetAllAsync(AttendanceParametrs attendanceParametrs);
    Task<PagedList<AttendanceBasicInfo>> GetByEmployeeIdAsync(string employeeId, AttendanceParametrs attendanceParametrs);
    Task<AttendanceRecord> CreateAsync(AttendanceRecord attendanceRecord);
    Task UpdateAsync(AttendanceRecord attendanceRecord);
    Task DeleteAsync(int id);
}
