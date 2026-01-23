namespace VITRACK.Api.DTOs.AttendanceRecords;

public class AttendanceBasicInfo
{
    public int Id { get; set; }
    public required string EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly? ArrivalTime { get; set; }
    public TimeOnly? LeaveTime { get; set; }
    public bool QrApprovedArrival { get; set; } = false;
    public bool QrApprovedLeave { get; set; } = false;
    public bool LocationApprovedArrival { get; set; } = false;
    public bool LocationApprovedLeave { get; set; } = false;
    public bool HasArrivalImage { get; set; } = false;
    public bool HasLeaveImage { get; set; } = false;
    public bool IsLate { get; set; } = false;
    public bool IsEarlyLeave { get; set; } = false;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}

