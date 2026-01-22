

using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.AttendanceRecords
{
    public class CheckInRequest
    {

        public TimeOnly? ArrivalTime { get; set; }

        public double? ArrivalLongitude { get; set; }
        public double? ArrivalLatitude { get; set; }

        public IFormFile? ArrivalImg { get; set; }

        [MaxLength(300)]
        public string? LateReason { get; set; }

    }
}