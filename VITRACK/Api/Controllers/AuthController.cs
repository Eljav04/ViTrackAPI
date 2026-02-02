using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VITRACK.Api.Errors;
using VITRACK.Infrastructure.Entities;
using VITRACK.Api.DTOs.Users;
using VITRACK.Common.Helpers;
using VITRACK.Api.DTOs.Auth;
using VITRACK.Common.Services;

namespace VITRACK.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    UserManager<User> _userManager;
    RoleManager<IdentityRole> _roleManager;
    SignInManager<User> _signInManager;
    IConfiguration _configuration;

    public AuthController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<User> signInManager,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ResponseErrors()
            {
                ErrorCodeSetter = ErrorCodeEnum.INPUT_ERROR,
                Message = ErrorCodes.INPUT_ERROR
            });
        }
        User? user = await _userManager.FindByNameAsync(userLogin.Login);

        if (user is null || user.IsDeleted)
        {
            return BadRequest(new ResponseErrors()
            {
                ErrorCodeSetter = ErrorCodeEnum.LOGIN_PASSWORD_ERROR,
                Message = ErrorCodes.LOGIN_PASSWORD_ERROR
            });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, userLogin.Password, true);

        if (result.Succeeded)
        {
            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            string? unitedUserRoles = string.Join(" ", userRoles);

            var jwt = JwtService.GenerateToken(user, unitedUserRoles, _configuration);
            Response.Cookies.Append("auth_token", jwt, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite = SameSiteMode.None,
                Secure = true,
            });

            return Ok();
        }

        if (result.IsLockedOut)
        {
            return BadRequest(
                new ResponseErrors()
                {
                    ErrorCodeSetter = ErrorCodeEnum.LOCKED_OUT_ERROR,
                    Message = ErrorCodes.LOCKED_OUT_ERROR
                });
        }

        return BadRequest(new ResponseErrors()
        {
            ErrorCodeSetter = ErrorCodeEnum.LOGIN_PASSWORD_ERROR,
            Message = ErrorCodes.LOGIN_PASSWORD_ERROR
        });
    }

    [HttpPost("logout")]
    public IActionResult LogOut()
    {
        var cookieOptions = new CookieOptions
        {
            // These MUST match the options used when creating the cookie
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        };

        // Pass the options to the Delete method
        Response.Cookies.Delete("auth_token", cookieOptions);

        return Ok(new
        {
            message = "success"
        });
    }
}