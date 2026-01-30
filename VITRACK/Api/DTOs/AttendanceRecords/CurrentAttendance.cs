
using System.ComponentModel.DataAnnotations;
using VITRACK.Common.Helpers;

namespace VITRACK.Api.DTOs.AttendanceRecords;

public class CurrentAttendance
{
    public DateOnly Date { get; set; }

    public TimeOnly? ArrivalTime { get; set; }
    public TimeOnly? LeaveTime { get; set; }

    public bool IsLate { get; set; } = false;
    public bool IsEarlyLeave { get; set; } = false;
    public bool IsRest { get; set; } = false;

}