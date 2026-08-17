namespace Experiments.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    IExperimentRepository Experiments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
