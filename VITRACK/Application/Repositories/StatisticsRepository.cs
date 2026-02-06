
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VITRACK.Api.DTOs.Statistics;
using VITRACK.Application.Interfaces;
using VITRACK.Infrastructure.Data;

namespace VITRACK.Application.Repositories;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly AppDbContext _context;

    public StatisticsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StatisticsDto> GetStatisticsByParamsAsync(string? employeeId = null, DateOnly? start = null, DateOnly? end = null, CancellationToken ct = default)
    {
        var query = _context.AttendanceRecords.AsQueryable();

        if (!string.IsNullOrWhiteSpace(employeeId))
            query = query.Where(ar => ar.EmployeeId == employeeId);

        if (start.HasValue)
            query = query.Where(ar => ar.Date >= start.Value);

        if (end.HasValue)
            query = query.Where(ar => ar.Date <= end.Value);

        // Get all data
        var data = await query
            .GroupBy(_ => 1)
            .Select(g => new
            {
                WorkDays = g.Count(ar => !ar.IsRest && !ar.IsAbsent),
                RestDays = g.Count(ar => ar.IsRest),
                AbsentDays = g.Count(ar => ar.IsAbsent),
                LateCount = g.Count(ar => ar.IsLate),
                EarlyLeaveCount = g.Count(ar => ar.IsEarlyLeave),
                WorkMinutes = g.Sum(ar => ar.AttendanceDurationMinutes ?? 0),
                OvertimeMinutes = g.Sum(ar => ar.OvertimeMinutes ?? 0),
                ExistingDates = g.Select(ar => ar.Date).Distinct()
            })
            .FirstOrDefaultAsync(ct);

        // If data is empty
        if (data == null || !start.HasValue || !end.HasValue)
        {
            return new StatisticsDto
            {
                MissingDates = new List<DateOnly>(),
                WholeDays = (start.HasValue && end.HasValue) ? end.Value.DayNumber - start.Value.DayNumber + 1 : 0
            };
        }

        // Calculate calendar dyas
        int calendarDays = end.Value.DayNumber - start.Value.DayNumber + 1;

        var allDates = Enumerable.Range(0, calendarDays)
            .Select(offset => start.Value.AddDays(offset))
            .ToList();

        // ist if missing days
        var missingDates = allDates.Except(data.ExistingDates).OrderBy(d => d).ToList();

        return new StatisticsDto
        {
            WholeDays = calendarDays,
            MissingDates = missingDates,
            WorkDays = data.WorkDays,
            RestDays = data.RestDays,
            AbsentDays = data.AbsentDays + missingDates.Count,
            LateCount = data.LateCount,
            EarlyLeaveCount = data.EarlyLeaveCount,
            WorkHours = data.WorkMinutes / 60.0,
            OvertimeHours = data.OvertimeMinutes / 60.0
        };
    }

    public async Task<ClippedStatisticsDto> GetClippedStatisticsByParamsAsync(string? employeeId = null, DateOnly? start = null, DateOnly? end = null, CancellationToken ct = default)
    {
        var query = _context.AttendanceRecords.AsQueryable();

        if (!string.IsNullOrWhiteSpace(employeeId))
            query = query.Where(ar => ar.EmployeeId == employeeId);

        if (start.HasValue)
            query = query.Where(ar => ar.Date >= start.Value);

        if (end.HasValue)
            query = query.Where(ar => ar.Date <= end.Value);


        var data = await query
            .GroupBy(_ => 1)
            .Select(g => new
            {
                WorkDays = g.Count(ar => !ar.IsRest && !ar.IsAbsent),
                RestDays = g.Count(ar => ar.IsRest),
                AbsentDays = g.Count(ar => ar.IsAbsent),
                LateCount = g.Count(ar => ar.IsLate),
                EarlyLeaveCount = g.Count(ar => ar.IsEarlyLeave),
                WorkMinutes = g.Sum(ar => ar.AttendanceDurationMinutes ?? 0),
                OvertimeMinutes = g.Sum(ar => ar.OvertimeMinutes ?? 0),
                ExistingDates = g.Select(ar => ar.Date).Distinct()
            })
            .FirstOrDefaultAsync(ct);

        if (data == null || !start.HasValue || !end.HasValue)
        {
            return new ClippedStatisticsDto
            {
                WholeDays = (start.HasValue && end.HasValue) ? end.Value.DayNumber - start.Value.DayNumber + 1 : 0
            };
        }

        int calendarDays = end.Value.DayNumber - start.Value.DayNumber + 1;


        var allDates = Enumerable.Range(0, calendarDays)
            .Select(offset => start.Value.AddDays(offset))
            .ToList();


        var missingDates = allDates.Except(data.ExistingDates).OrderBy(d => d).ToList();

        return new ClippedStatisticsDto
        {
            WholeDays = calendarDays,
            WorkDays = data.WorkDays,
            RestDays = data.RestDays,
            AbsentDays = data.AbsentDays + missingDates.Count,
            LateCount = data.LateCount,
            EarlyLeaveCount = data.EarlyLeaveCount,
            WorkHours = data.WorkMinutes / 60.0,
            OvertimeHours = data.OvertimeMinutes / 60.0
        };
    }

}
