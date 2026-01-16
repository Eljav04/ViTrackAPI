namespace VITRACK.Api.DTOs.WorkSchedules;

public class WorkScheduleReadDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
