namespace AccountService.Application.Common.Interfaces;

/// <summary>
/// Interface cho dịch vụ DateTime
/// </summary>
public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}


