using AIAnalysis.Application.Interfaces;

namespace AIAnalysis.Application.Common;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
