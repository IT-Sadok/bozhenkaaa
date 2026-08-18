namespace Experiments.Application.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
