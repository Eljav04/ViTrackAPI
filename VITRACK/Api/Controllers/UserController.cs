using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VITRACK.Api.Errors;
using VITRACK.Infrastructure.Entities;
using VITRACK.Api.DTOs.Users;
using VITRACK.Common.Helpers;
using Azure;

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
}