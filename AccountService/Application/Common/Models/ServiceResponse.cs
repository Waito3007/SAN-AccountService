namespace AccountService.Application.Common.Models;

/// <summary>
/// Phản hồi chuẩn cho các service trong Application layer.
/// </summary>
public class ServiceResponse
{
    public bool Succeeded { get; set; }

    public string? Message { get; set; }

    public List<string> Errors { get; set; } = new();

    public static ServiceResponse Success(string? message = null)
    {
        return new ServiceResponse { Succeeded = true, Message = message };
    }

    public static ServiceResponse Failure(params string[] errors)
    {
        return new ServiceResponse { Succeeded = false, Errors = errors.ToList() };
    }
}

/// <summary>
/// Phản hồi chuẩn có dữ liệu.
/// </summary>
public class ServiceResponse<T> : ServiceResponse
{
    public T? Data { get; set; }

    public new static ServiceResponse<T> Success(T data, string? message = null)
    {
        return new ServiceResponse<T> { Succeeded = true, Data = data, Message = message };
    }

    public new static ServiceResponse<T> Failure(params string[] errors)
    {
        return new ServiceResponse<T> { Succeeded = false, Errors = errors.ToList() };
    }
}
