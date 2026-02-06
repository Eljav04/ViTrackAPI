

using System.Threading;
using System.Threading.Tasks;
using VITRACK.Api.DTOs.Statistics;

namespace VITRACK.Application.Interfaces;

public interface IStatisticsRepository
{
    Task<StatisticsDto> GetStatisticsByParamsAsync(string? employeeId = null, DateOnly? start = null, DateOnly? end = null, CancellationToken ct = default);
    Task<ClippedStatisticsDto> GetClippedStatisticsByParamsAsync(string? employeeId = null, DateOnly? start = null, DateOnly? end = null, CancellationToken ct = default);
}
