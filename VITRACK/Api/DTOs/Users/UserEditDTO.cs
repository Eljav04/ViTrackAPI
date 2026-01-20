using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.Users;

public class UserEditDTO
{
    [Required]
    public required string Id { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 1)]
    [RegularExpression(@"^[A-Za-zА-Яа-яЁёÇçƏəÖöŞşÜüİIıĞğ]+$")]
    public required string Firstname { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    [RegularExpression(@"^[A-Za-zА-Яа-яЁёÇçƏəÖöŞşÜüİIıĞğ]+$")]
    public required string Lastname { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 5)]
    public required string Login { get; set; }

    public int? DepartmentId { get; set; }
    public int? WorkScheduleId { get; set; }


}