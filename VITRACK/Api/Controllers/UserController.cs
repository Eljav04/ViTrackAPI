using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VITRACK.Api.Errors;
using VITRACK.Infrastructure.Entities;
using VITRACK.Api.DTOs.Users;
using VITRACK.Common.Helpers;
using System.Security.Claims;
using VITRACK.Api.DTOs.Auth;
using VITRACK.Common.Services;
using Microsoft.AspNetCore.Authorization;
using VITRACK.Application.Interfaces;

namespace VITRACK.Api.Controllers;

[Route("api/user")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    UserManager<User> _userManager { get; set; }
    IUserRepository _userRepository { get; set; }

    public UserController(
        UserManager<User> userManager,
        IUserRepository userRepository
    )
    {
        _userRepository = userRepository;
        _userManager = userManager;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;
        UserInfo? info = JwtService.GetCurrentUserInfo(identity);
        if (info?.Id is null) return StatusCode(500, new ResponseErrors
        {
            ErrorCodeSetter = ErrorCodeEnum.INTERNAL_SERVER_ERROR,
            Message = ErrorCodes.INTERNAL_SERVER_ERROR
        });

        User? user = await _userManager.FindByIdAsync(info.Id);

        if (user is null || user.IsDeleted)
        {
            return BadRequest();
        }

        UserMeDTO userMeDTO = new()
        {
            Firstname = user.Firstname,
            Lastname = user.Surname,
            Role = info.Role
        };

        return Ok(userMeDTO);

    }


    [HttpGet("all")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAllUsers([FromQuery] bool IsDeleted = false)
    {
        var users = _userManager.Users.Where(u => u.IsDeleted == IsDeleted).ToList();
        var result = new List<UserMeDTO>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            result.Add(new UserMeDTO
            {
                Firstname = user.Firstname,
                Lastname = user.Surname,
                Role = role
            });
        }

        return Ok(result);
    }

    [HttpGet("all-detailed")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAllDetailedUsers([FromQuery] bool IsDeleted = false)
    {
        var users = await _userRepository.GetAllUsersWithDetailsAsync(IsDeleted);
        return Ok(users);
    }


    [HttpPost("registr")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Registr([FromBody] UserRegistrDTO userRegistrDTO)
    {
        User? existingUser = await _userManager.FindByNameAsync(userRegistrDTO.Login);
        if (existingUser != null)
        {
            return BadRequest(new ResponseErrors
            {
                ErrorCodeSetter = ErrorCodeEnum.USER_ALREADY_EXISTS,
                Message = ErrorCodes.USER_ALREADY_EXISTS
            });
        }

        User newUser = new()
        {
            UserName = userRegistrDTO.Login,
            Firstname = userRegistrDTO.Firstname,
            Surname = userRegistrDTO.Lastname,
            CreationTime = TimeHelper.GetBakuTime(),
        };

        var result = await _userManager.CreateAsync(newUser, userRegistrDTO.Password);

        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(newUser, Roles.User);
            if (!roleResult.Succeeded)
            {
                return StatusCode(500, new ResponseErrors
                {
                    ErrorCodeSetter = ErrorCodeEnum.INTERNAL_SERVER_ERROR,
                    Message = ErrorCodes.INTERNAL_SERVER_ERROR
                });
            }

            return Ok("Istifadəçi uğurla yaradıldı.");
        }

        return BadRequest(new ResponseErrors
        {
            ErrorCodeSetter = ErrorCodeEnum.UNXEPECTED_ERROR,
            Message = ErrorCodes.UNXEPECTED_ERROR
        });

    }

    [HttpPost("registr-admin")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> RegistrAdmin([FromBody] UserRegistrDTO userRegistrDTO)
    {
        User? existingUser = await _userManager.FindByNameAsync(userRegistrDTO.Login);
        if (existingUser != null)
        {
            return BadRequest(new ResponseErrors
            {
                ErrorCodeSetter = ErrorCodeEnum.USER_ALREADY_EXISTS,
                Message = ErrorCodes.USER_ALREADY_EXISTS
            });
        }

        User newUser = new()
        {
            UserName = userRegistrDTO.Login,
            Firstname = userRegistrDTO.Firstname,
            Surname = userRegistrDTO.Lastname,
            CreationTime = TimeHelper.GetBakuTime(),
        };

        var result = await _userManager.CreateAsync(newUser, userRegistrDTO.Password);

        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(newUser, Roles.Admin);
            if (!roleResult.Succeeded)
            {
                return StatusCode(500, new ResponseErrors
                {
                    ErrorCodeSetter = ErrorCodeEnum.INTERNAL_SERVER_ERROR,
                    Message = ErrorCodes.INTERNAL_SERVER_ERROR
                });
            }

            return Ok("Istifadəçi uğurla yaradıldı.");
        }

        return BadRequest(new ResponseErrors
        {
            ErrorCodeSetter = ErrorCodeEnum.UNXEPECTED_ERROR,
            Message = ErrorCodes.UNXEPECTED_ERROR
        });
    }

    [HttpPut("change-password/{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ChangePassword([FromRoute] string id, [FromBody] string NewPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound(new ResponseErrors
            {
                ErrorCodeSetter = ErrorCodeEnum.USER_NOT_FOUND,
                Message = ErrorCodes.USER_NOT_FOUND
            });
        }

        var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, NewPassword);
        user.PasswordHash = newPasswordHash;
        await _userManager.UpdateAsync(user);

        return Ok("Parol uğurla dəyişdirildi.");
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null || user.IsDeleted)
        {
            return NotFound(new ResponseErrors
            {
                ErrorCodeSetter = ErrorCodeEnum.USER_NOT_FOUND,
                Message = ErrorCodes.USER_NOT_FOUND
            });
        }

        user.IsDeleted = true;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    [HttpPut("restore/{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> RestoreUser([FromRoute] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null || !user.IsDeleted)
        {
            return NotFound(new ResponseErrors
            {
                ErrorCodeSetter = ErrorCodeEnum.USER_NOT_FOUND,
                Message = ErrorCodes.USER_NOT_FOUND
            });
        }

        user.IsDeleted = false;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }



}