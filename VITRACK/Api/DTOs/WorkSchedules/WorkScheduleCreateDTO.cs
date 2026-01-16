using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.WorkSchedules;

public class WorkScheduleCreateDTO
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }
}
