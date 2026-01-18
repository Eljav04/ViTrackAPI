

using System.ComponentModel.DataAnnotations;

namespace VITRACK.Api.DTOs.Auth
{
    public class UserLogin
    {
        [Required]
        public required string Login { get; set; }
        [Required]
        public required string Password { get; set; }

    }
}