using Microsoft.AspNetCore.Mvc;
using VITRACK.Api.DTOs.Departments;
using VITRACK.Api.Errors;
using VITRACK.Application.Interfaces;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Api.Controllers;

[Route("api/department")]
[ApiController]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentRepository _repository;

    public DepartmentController(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var deps = await _repository.GetAllAsync();
        var result = deps.Select(d => new DepartmentReadDTO { Id = d.Id, Name = d.Name });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var dep = await _repository.GetByIdAsync(id);
        if (dep is null)
            return NotFound(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.NOT_FOUND, Message = "Departament tapılmadı." });

        return Ok(new DepartmentReadDTO { Id = dep.Id, Name = dep.Name });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DepartmentCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.INPUT_ERROR, Message = ErrorCodes.INPUT_ERROR });

        var department = new Department { Name = dto.Name };
        var created = await _repository.CreateAsync(department);

        return CreatedAtAction(nameof(Get), new { id = created.Id }, new DepartmentReadDTO { Id = created.Id, Name = created.Name });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DepartmentUpdateDTO dto)
    {
        if (id != dto.Id)
            return BadRequest(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.INPUT_ERROR, Message = ErrorCodes.INPUT_ERROR });

        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.NOT_FOUND, Message = "Departament tapılmadı." });

        existing.Name = dto.Name;
        await _repository.UpdateAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new ResponseErrors { ErrorCodeSetter = ErrorCodeEnum.NOT_FOUND, Message = "Departament tapılmadı." });

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
