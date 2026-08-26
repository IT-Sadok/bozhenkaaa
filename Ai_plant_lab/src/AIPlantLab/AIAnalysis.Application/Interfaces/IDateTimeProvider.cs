namespace AIAnalysis.Application.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
