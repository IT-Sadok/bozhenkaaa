using Experiments.Application.Interfaces;

namespace Experiments.Application.Common;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
