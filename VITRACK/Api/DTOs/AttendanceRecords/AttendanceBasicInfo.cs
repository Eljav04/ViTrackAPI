namespace VITRACK.Api.DTOs.AttendanceRecords;

public class AttendanceBasicInfo
{
    public int Id { get; set; }
    public EmployeeBasicInfo? Employee { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly? ArrivalTime { get; set; }
    public TimeOnly? LeaveTime { get; set; }
    public bool QrApprovedArrival { get; set; } = false;
    public bool QrApprovedLeave { get; set; } = false;
    public bool LocationApprovedArrival { get; set; } = false;
    public LocationBasicInfo? ArrivalLocation { get; set; }
    public bool LocationApprovedLeave { get; set; } = false;
    public LocationBasicInfo? LeaveLocation { get; set; }
    public string? ArrivalImage { get; set; }
    public string? LeaveImage { get; set; }
    public string? LateReason { get; set; }
    public string? EarlyLeaveReason { get; set; }
    public bool IsLate { get; set; } = false;
    public bool IsEarlyLeave { get; set; } = false;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}


public class EmployeeBasicInfo
{
    public string? Id { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? DepartmentName { get; set; }
}

public class LocationBasicInfo
{
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
}