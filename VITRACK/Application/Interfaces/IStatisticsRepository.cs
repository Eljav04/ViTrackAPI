

using System.Threading;
using System.Threading.Tasks;
using VITRACK.Application.DTOs;

namespace VITRACK.Application.Interfaces;

public interface IStatisticsRepository
{
    Task<StatisticsDto> GetStatisticsByParamsAsync(string? employeeId = null, DateOnly? start = null, DateOnly? end = null, CancellationToken ct = default);
}
