
using VITRACK.Application.Interfaces;
using VITRACK.Infrastructure.Data;

namespace VITRACK.Application.Repositories;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly AppDbContext _context;

    public StatisticsRepository(AppDbContext context)
    {
        _context = context;
    }

}
