using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VITRACK.Api.Errors;
using VITRACK.Infrastructure.Entities;
using VITRACK.Api.DTOs.Users;
using VITRACK.Common.Helpers;
using System.Security.Claims;
using VITRACK.Api.DTOs.Auth;
using VITRACK.Common.Services;

namespace VITRACK.Api.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    UserManager<User> _userManager { get; set; }

    public UserController(
        UserManager<User> userManager
    )
    {
        _userManager = userManager;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;
        UserInfo? info = JwtService.GetCurrentUserInfo(identity);

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

    [HttpPost("registr")]
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


}