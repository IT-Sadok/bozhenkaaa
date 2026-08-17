using Experiments.Domain.Entities;

namespace Experiments.Application.Interfaces.Repositories;

public interface IExperimentRepository
{
    Task<Experiment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(Experiment experiment);

    Task<(IReadOnlyList<Experiment> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
