using System.Collections.Generic;
using System.Threading.Tasks;
using VITRACK.Api.DTOs.Users;

namespace VITRACK.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<UserFullDataDTO>> GetAllUsersWithDetailsAsync(bool? isDeleted = false, string? roleName = null);

    Task<UserFullDataDTO?> GetUserWithDetailsByIdAsync(string id);

    Task UpdateAsync(UserEditDTO userDto);
}
