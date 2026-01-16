using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.WorkSchedules;

public class WorkScheduleAssignUserBodyDTO
{
    [Required]
    public int WorkScheduleId { get; set; }

    [Required]
    public required string UserId { get; set; }
}
