using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Api.Controllers;

[Route("api/[controller]")]
public class UserController : ControllerBase
{
    UserManager<User> _userManager { get; set; }

    public UserController(
        UserManager<User> userManager
    )
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok("Work");
    }


}