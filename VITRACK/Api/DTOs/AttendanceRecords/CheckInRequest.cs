

using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.AttendanceRecords
{
    public class CheckInRequest
    {

        public TimeOnly? ArrivalTime { get; set; }

        public string? ArrivalLongitude { get; set; }
        public string? ArrivalLatitude { get; set; }

        public IFormFile? ArrivalImg { get; set; }

        [MaxLength(300)]
        public string? LateReason { get; set; }

    }
}