using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace VITRACK.Infrastructure.Entities;

public sealed class User : IdentityUser
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    [RegularExpression(@"^[A-Za-zА-Яа-яЁёÇçƏəÖöŞşÜüİIıĞğ'-]+$")]
    public required string Firstname { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 1)]
    [RegularExpression(@"^[A-Za-zА-Яа-яЁёÇçƏəÖöŞşÜüİIıĞğ'-]+$")]
    public required string Surname { get; set; }
    public string? ProfileImgPath { get; set; }

    public DateTime? CreationTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    // After deleting user this value must be true
    public bool IsDeleted { get; set; } = false;

    // === Navigation Properties ===
    public Department? Department { get; set; }
    public int? DepartmentId { get; set; }

    public WorkSchedule? WorkSchedule { get; set; }
    public int? WorkScheduleId { get; set; }
    // === Navigation Properties ===


}