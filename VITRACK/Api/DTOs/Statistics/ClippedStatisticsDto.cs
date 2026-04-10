using System;

namespace VITRACK.Api.DTOs.Statistics;

public sealed class ClippedStatisticsDto
{
    public int WholeDays { get; set; }
    public int WorkDays { get; set; }
    public int RestDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateCount { get; set; }
    public int EarlyLeaveCount { get; set; }
    public double WorkHours { get; set; }
    public double OvertimeHours { get; set; }
}
