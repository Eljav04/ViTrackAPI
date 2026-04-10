
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

    public async Task<TodayOvevallStatsDto> GetTodayOverallStatisticsAsync(DateOnly date, CancellationToken ct = default)
    {
        // 1. Get employees (users with role 'User')
        var usersQuery = from u in _context.Users
                         join ur in _context.UserRoles on u.Id equals ur.UserId
                         join r in _context.Roles on ur.RoleId equals r.Id
                         where r.Name == Infrastructure.Entities.Roles.User && !u.IsDeleted
                         select new { u.Id, u.Firstname, u.Surname, u.Department };

        var users = await usersQuery.ToListAsync(ct);

        if (!users.Any())
            return new TodayOvevallStatsDto();

        var userIds = users.Select(u => u.Id).ToList();

        // 2. Get today's attendance records for these users
        var records = await _context.AttendanceRecords
            .Where(ar => userIds.Contains(ar.EmployeeId) && ar.Date == date)
            .ToListAsync(ct);

        // 3. Process data
        var employeesList = new List<EmployeeBasicInfo>();
        var departmentStatsMap = new Dictionary<string, DepartmentStats>();

        int totalEmployees = users.Count;
        int presentEmployees = 0; // Has record
        int absentEmployees = 0;  // IsAbsent = true
        int restEmployees = 0;    // IsRest = true
        int lateArrivalsCount = 0; // IsLate = true

        foreach (var user in users)
        {
            var record = records.FirstOrDefault(r => r.EmployeeId == user.Id);
            
            // Department handling
            string deptName = user.Department?.Name ?? "Digər";
            int deptId = user.Department?.Id ?? 0;

            if (!departmentStatsMap.ContainsKey(deptName))
            {
                departmentStatsMap[deptName] = new DepartmentStats 
                { 
                    Id = deptId, 
                    Name = deptName,
                    TotalEmployees = 0,
                    PresentEmployees = 0,
                    LateCount = 0
                };
            }
            
            var deptStats = departmentStatsMap[deptName];
            deptStats.TotalEmployees++;

            // Status checks
            bool hasRecord = record != null;
            bool isRest = record?.IsRest ?? false;
            bool isLate = record?.IsLate ?? false;
            
            // User Change: If logic for employee not exist record for today -> IsAbsent = true
            // OR if record exist but IsAbsent = true -> IsAbsent = true
            bool isAbsent = !hasRecord || (record?.IsAbsent ?? false);

            if (isAbsent)
            {
                absentEmployees++;
            }
            else if (isRest)
            {
                restEmployees++;
            }
            else
            {
                // Present (Has record, Not Absent, Not Rest)
                presentEmployees++;
                deptStats.PresentEmployees++;

                if (isLate)
                {
                    lateArrivalsCount++;
                    deptStats.LateCount++;
                }
            }

            // Employee List Item
            var employeeInfo = new EmployeeBasicInfo
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Surname,
                DepartmentName = deptName,
                CheckInTime = record?.ArrivalTime,
                IsLate = isLate,
                IsRest = isRest,
                IsAbsent = isAbsent
            };
            employeesList.Add(employeeInfo);
        }

        return new TodayOvevallStatsDto
        {
            TotalEmployees = totalEmployees,
            PresentEmployees = presentEmployees,
            AbsentEmployees = absentEmployees,
            RestEmployees = restEmployees,
            LateArrivalsCount = lateArrivalsCount,
            EmployeesList = employeesList
                .OrderByDescending(e => e.IsAbsent)
                .ThenByDescending(e => e.IsLate)
                .ToList(),
            DepartmentsList = departmentStatsMap.Values.ToList()
        };
    }

}
