using System.Collections.Generic;
using System.Threading.Tasks;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Application.Interfaces;

public interface IAttendanceRecordRepository
{
    Task<AttendanceRecord?> GetByDateAsync(string employeeId, DateOnly date);
    Task<AttendanceRecord?> GetByIdAsync(int id);
    Task<AttendanceRecord> CreateAsync(AttendanceRecord attendanceRecord);
    Task UpdateAsync(AttendanceRecord attendanceRecord);
    Task DeleteAsync(int id);
}
