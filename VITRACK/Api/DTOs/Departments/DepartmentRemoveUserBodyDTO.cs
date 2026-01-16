using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.Departments;

public class DepartmentRemoveUserBodyDTO
{
    [Required]
    public int DepartmentId { get; set; }

    [Required]
    public required string UserId { get; set; }
}
