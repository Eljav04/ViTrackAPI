using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.WorkSchedules;

public class WorkScheduleRemoveUserBodyDTO
{
    [Required]
    public int WorkScheduleId { get; set; }

    [Required]
    public required string UserId { get; set; }
}
