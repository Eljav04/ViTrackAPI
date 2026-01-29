
using System.ComponentModel.DataAnnotations;

namespace VITRACK.Infrastructure.Entities;

public sealed class Test
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string TestName { get; set; }
}