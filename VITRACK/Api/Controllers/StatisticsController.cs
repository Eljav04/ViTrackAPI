using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITRACK.Application.Interfaces;
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

}
