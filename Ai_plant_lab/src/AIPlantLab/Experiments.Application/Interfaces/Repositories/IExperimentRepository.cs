using Experiments.Application.Common;
using Experiments.Domain.Entities;

namespace Experiments.Application.Interfaces.Repositories;

public interface IExperimentRepository
{
    Task<Experiment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(Experiment experiment);

    Task<PagedResult<Experiment>> ListAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
