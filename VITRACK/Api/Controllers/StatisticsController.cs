using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITRACK.Api.DTOs.Auth;
using VITRACK.Application.Interfaces;
using VITRACK.Common.Helpers;
using VITRACK.Common.Services;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Api.Controllers;

[Route("api/statistics")]
[ApiController]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsRepository _repository;

    public StatisticsController(IStatisticsRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("attendace/get-stats-by-employee")]
    [Authorize(Roles = "Admin,Boss")]
    public async Task<IActionResult> GetStatisticsByEmployee([FromQuery] string? employeeId = null
        , [FromQuery] DateOnly? start = null
        , [FromQuery] DateOnly? end = null
        , CancellationToken ct = default)
    {
        if (start is null)
            return BadRequest("Başlanğıc tarixi göstərilməyib.");
        if (end is null)
            end = TimeHelper.GetBakuDate();
        if (end < start)
            return BadRequest("Tarix aralığı yalnışdır. Baş tarix son tarixden böyük ola bilməz.");

        var stats = await _repository.GetStatisticsByParamsAsync(employeeId, start, end, ct);

        return Ok(stats);
    }

    [HttpGet("attendace/get-my-stats")]
    [Authorize(Roles = Roles.User)]
    public async Task<IActionResult> GetMyStatistics([FromQuery] DateOnly? start = null
        , [FromQuery] DateOnly? end = null
        , CancellationToken ct = default)
    {
        if (start is null)
            return BadRequest("Başlanğıc tarixi göstərilməyib.");
        if (end is null)
            end = TimeHelper.GetBakuDate();
        if (end < start)
            return BadRequest("Tarix aralığı yalnışdır. Baş tarix son tarixden böyük ola bilməz.");

        UserInfo? userInfo =
             JwtService.GetCurrentUserInfo(HttpContext.User.Identity as ClaimsIdentity);

        if (userInfo?.Id is null) return StatusCode(500);

        var stats = await _repository.GetStatisticsByParamsAsync(userInfo.Id, start, end, ct);
        return Ok(stats);
    }
}