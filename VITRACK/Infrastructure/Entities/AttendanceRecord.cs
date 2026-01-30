
using System.ComponentModel.DataAnnotations;
using VITRACK.Common.Helpers;

namespace VITRACK.Infrastructure.Entities;

public sealed class AttendanceRecord
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(450)]
    public required string EmployeeId { get; set; }
    [Required]
    public DateOnly Date { get; set; }

    public TimeOnly? ArrivalTime { get; set; }
    public TimeOnly? LeaveTime { get; set; }

    public double? ArrivalLongitude { get; set; }
    public double? ArrivalLatitude { get; set; }
    public double? LeaveLongitude { get; set; }
    public double? LeaveLatitude { get; set; }

    [MaxLength(1000)]
    public string? ArrivalImgUrl { get; set; }
    [MaxLength(1000)]
    public string? LeaveImgUrl { get; set; }

    [MaxLength(300)]
    public string? LateReason { get; set; }
    [MaxLength(300)]
    public string? EarlyLeaveReason { get; set; }

    public bool QrApprovedArrival { get; set; } = false;
    public bool QrApprovedLeave { get; set; } = false;
    public bool IsLate { get; set; } = false;
    public bool IsEarlyLeave { get; set; } = false;
    public bool IsAbsent { get; set; } = false;
    public bool IsRest { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = TimeHelper.GetBakuTime();
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public User? Employee { get; set; }

}