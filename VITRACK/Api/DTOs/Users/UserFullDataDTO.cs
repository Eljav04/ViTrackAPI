namespace VITRACK.Api.DTOs.Users;

public class DepartmentSimpleDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class WorkScheduleSimpleDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class UserFullDataDTO
{
    public string? Id { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Role { get; set; }
    public string? Login { get; set; }
    public DepartmentSimpleDTO? Department { get; set; }
    public WorkScheduleSimpleDTO? WorkSchedule { get; set; }
}
