using System;

namespace VITRACK.Application.DTOs;

public sealed class StatisticsDto
{
    public int WholeDays { get; set; }
    public int WorkDays { get; set; }
    public int RestDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateCount { get; set; }
    public int EarlyLeaveCount { get; set; }
    public double WorkHours { get; set; }
    public double OvertimeHours { get; set; }
    public List<DateOnly> MissingDates { get; set; } = new();
}
