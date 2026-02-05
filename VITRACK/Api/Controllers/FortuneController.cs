using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace VITRACK.Api.Controllers;

[Route("api/fortune")]
[ApiController]
public class FortuneController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public FortuneController(IWebHostEnvironment env)
    {
        _env = env;
    }

    private string WheelDirectory => Path.Combine(_env.WebRootPath ?? "wwwroot", "fortune");
    private string WheelFilePath => Path.Combine(WheelDirectory, "wheel_items.json");

    [HttpGet("wheel-items")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult GetWheelItems()
    {
        if (!System.IO.File.Exists(WheelFilePath))
        {
            return NotFound();
        }

        var json = System.IO.File.ReadAllText(WheelFilePath);
        return Content(json, "application/json");
    }

    [HttpPost("wheel-items")]
    public async Task<IActionResult> UpsertWheelItems([FromBody] JsonElement payload)
    {
        Directory.CreateDirectory(WheelDirectory);
        await System.IO.File.WriteAllTextAsync(WheelFilePath, payload.GetRawText());
        return Ok(new { success = true });
    }
}

