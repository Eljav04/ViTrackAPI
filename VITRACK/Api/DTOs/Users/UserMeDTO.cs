using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.Users;

public class UserMeDTO
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Role { get; set; }

}