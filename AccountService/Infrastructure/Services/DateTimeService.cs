using AccountService.Application.Common.Interfaces;

namespace AccountService.Infrastructure.Services;

/// <summary>
/// Service cung cấp DateTime
/// </summary>
public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}

