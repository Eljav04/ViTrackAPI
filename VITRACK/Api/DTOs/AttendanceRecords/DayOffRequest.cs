
using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.AttendanceRecords
{
    public class DayOffRequest
    {
        [MaxLength(300)]
        public string? Reason { get; set; }
    }
}