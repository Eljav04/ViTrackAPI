using System;

namespace VITRACK.Api.DTOs.Statistics;

public sealed class TodayOvevallStatsDto
{
    public int TotalEmployees { get; set; }
    public int PresentEmployees { get; set; }
    public int AbsentEmployees { get; set; }
    public int RestEmployees { get; set; }
    public int LateArrivalsCount { get; set; }
    public List<EmployeeBasicInfo> EmployeesList { get; set; } = new();
    public List<DepartmentStats> DepartmentsList { get; set; } = new();
}

public class EmployeeBasicInfo
{
    public string? Id { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? DepartmentName { get; set; }
    public TimeOnly? CheckInTime { get; set; }
    public bool IsLate { get; set; } = false;
    public bool IsRest { get; set; } = false;
    public bool IsAbsent { get; set; } = false;
}

public class DepartmentStats
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int TotalEmployees { get; set; }
    public int PresentEmployees { get; set; }
    public int LateCount { get; set; }
}