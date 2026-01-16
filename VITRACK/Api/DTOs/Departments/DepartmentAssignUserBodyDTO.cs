using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.Departments;

public class DepartmentAssignUserBodyDTO
{
    [Required]
    public int DepartmentId { get; set; }

    [Required]
    public required string UserId { get; set; }
}
