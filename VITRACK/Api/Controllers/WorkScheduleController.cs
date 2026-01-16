using Microsoft.AspNetCore.Mvc;
using VITRACK.Api.DTOs.WorkSchedules;
using VITRACK.Api.Errors;
using VITRACK.Application.Interfaces;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Api.Controllers;

[Route("api/workschedule")]
[ApiController]
public class WorkScheduleController : ControllerBase
{
    private readonly IWorkScheduleRepository _repository;

    public WorkScheduleController(IWorkScheduleRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _repository.GetAllAsync();
        var result = items.Select(w => new WorkScheduleReadDTO { Id = w.Id, Name = w.Name, StartTime = w.StartTime, EndTime = w.EndTime });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var ws = await _repository.GetByIdAsync(id);
        if (ws is null)
            return NotFound(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.NOT_FOUND, Message = "WorkSchedule not found." });

        return Ok(new WorkScheduleReadDTO { Id = ws.Id, Name = ws.Name, StartTime = ws.StartTime, EndTime = ws.EndTime });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkScheduleCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.INPUT_ERROR, Message = ErrorCodes.INPUT_ERROR });

        var ws = new WorkSchedule { Name = dto.Name, StartTime = dto.StartTime, EndTime = dto.EndTime };
        var created = await _repository.CreateAsync(ws);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, new WorkScheduleReadDTO { Id = created.Id, Name = created.Name, StartTime = created.StartTime, EndTime = created.EndTime });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] WorkScheduleUpdateDTO dto)
    {
        if (id != dto.Id)
            return BadRequest(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.INPUT_ERROR, Message = ErrorCodes.INPUT_ERROR });

        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.NOT_FOUND, Message = "WorkSchedule not found." });

        existing.Name = dto.Name;
        existing.StartTime = dto.StartTime;
        existing.EndTime = dto.EndTime;
        await _repository.UpdateAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.NOT_FOUND, Message = "WorkSchedule not found." });

        await _repository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("assign-user")]
    public async Task<IActionResult> AssignUser([FromBody] WorkScheduleAssignUserBodyDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.INPUT_ERROR, Message = ErrorCodes.INPUT_ERROR });

        var ws = await _repository.GetByIdAsync(dto.WorkScheduleId);
        if (ws is null)
            return NotFound(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.NOT_FOUND, Message = "WorkSchedule not found." });

        var success = await _repository.AssignUserToWorkScheduleAsync(dto.WorkScheduleId, dto.UserId);
        if (!success)
            return NotFound(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.USER_NOT_FOUND, Message = ErrorCodes.USER_NOT_FOUND });

        return NoContent();
    }

    [HttpDelete("remove-user")]
    public async Task<IActionResult> RemoveUser([FromBody] WorkScheduleRemoveUserBodyDTO dto)
    {
        var ws = await _repository.GetByIdAsync(dto.WorkScheduleId);
        if (ws is null)
            return NotFound(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.NOT_FOUND, Message = "WorkSchedule not found." });

        var success = await _repository.RemoveUserFromWorkScheduleAsync(dto.WorkScheduleId, dto.UserId);
        if (!success)
            return NotFound(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.USER_NOT_FOUND, Message = ErrorCodes.USER_NOT_FOUND });

        return NoContent();
    }
}
