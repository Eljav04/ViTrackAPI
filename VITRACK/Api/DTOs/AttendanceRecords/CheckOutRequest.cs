

using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.AttendanceRecords
{
    public class CheckOutRequest
    {

        public TimeOnly? LeaveTime { get; set; }

        public double? LeaveLongitude { get; set; }
        public double? LeaveLatitude { get; set; }

        public IFormFile? LeaveImg { get; set; }

        [MaxLength(300)]
        public string? EarlyLeaveReason { get; set; }

    }
}