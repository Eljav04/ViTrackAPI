using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VITRACK.Api.Errors;
using VITRACK.Infrastructure.Entities;

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

    [HttpGet("registr")]
    public async Task<IActionResult> Registr()
    {
        // return Ok("Work");
        return Ok(new ResponseErrors
        {
            ErrorCodeSetter = ErrorCodeEnum.INPUT_ERROR,
            Message = ErrorCodes.INPUT_ERROR
        });
    }


}