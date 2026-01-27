

using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.AttendanceRecords
{
    public class CheckOutRequest
    {

        public TimeOnly? LeaveTime { get; set; }

        public string? LeaveLongitude { get; set; }
        public string? LeaveLatitude { get; set; }

        public IFormFile? LeaveImg { get; set; }

        [MaxLength(300)]
        public string? EarlyLeaveReason { get; set; }

    }
}