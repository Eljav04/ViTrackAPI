using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITRACK.Application.Interfaces;

namespace VITRACK.Api.Controllers;

[Route("api/attendance-record")]
[ApiController]
[Authorize]
public class AttendanceRecordController : ControllerBase
{
    private readonly IAttendanceRecordRepository _repository;

    public AttendanceRecordController(IAttendanceRecordRepository repository)
    {
        _repository = repository;
    }

    // [HttpPost("chek-in")]
    // public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
    // {
    //     return Ok(await _repository.CheckInAsync(request));
    // }

}
