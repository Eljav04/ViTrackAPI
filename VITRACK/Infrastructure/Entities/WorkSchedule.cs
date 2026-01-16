using System.ComponentModel.DataAnnotations;

namespace VITRACK.Infrastructure.Entities;

public sealed class WorkSchedule
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; set; }
    [Required]
    public TimeOnly StartTime { get; set; }
    [Required]
    public TimeOnly EndTime { get; set; }
}