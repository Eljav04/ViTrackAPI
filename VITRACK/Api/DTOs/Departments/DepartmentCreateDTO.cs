using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.Departments;

public class DepartmentCreateDTO
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; set; }
}
