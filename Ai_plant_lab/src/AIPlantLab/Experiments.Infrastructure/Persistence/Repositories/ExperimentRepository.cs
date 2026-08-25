using Experiments.Application.Common;
using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Experiments.Infrastructure.Persistence.Repositories;

public sealed class ExperimentRepository(AppDbContext dbContext) : IExperimentRepository
{
    public async Task<Experiment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Experiments
            .Include(e => e.PlantGroups)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public void Add(Experiment experiment)
    {
        dbContext.Experiments.Add(experiment);
    }

    public async Task<PagedResult<Experiment>> ListAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Experiments
            .Include(e => e.PlantGroups)
            .OrderByDescending(e => e.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Experiment>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
