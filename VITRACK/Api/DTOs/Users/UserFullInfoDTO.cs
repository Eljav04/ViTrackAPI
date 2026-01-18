using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.Users;

public class UserFullInfoDTO
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Role { get; set; }
    public required string Login { get; set; }

}