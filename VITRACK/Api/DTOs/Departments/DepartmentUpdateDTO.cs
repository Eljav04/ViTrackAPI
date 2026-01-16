using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.Departments;

public class DepartmentUpdateDTO
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; set; }
}
