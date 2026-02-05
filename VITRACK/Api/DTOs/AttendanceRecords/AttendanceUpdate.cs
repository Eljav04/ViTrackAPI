
using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.AttendanceRecords
{
    public class AttendanceUpdate
    {
        public int Id { get; set; }
        public TimeOnly? ArrivalTime { get; set; }
        public TimeOnly? LeaveTime { get; set; }
        public bool? IsLate { get; set; }
        public bool? IsEarlyLeave { get; set; } = false;
        public bool? IsAbsent { get; set; } = false;
        public bool? IsRest { get; set; } = false;

    }
}