namespace AccountService.Application.Common.Models;

/// <summary>
/// Kết quả trả về từ Application layer
/// </summary>
public class Result
{
    public bool Succeeded { get; set; }
    public bool IsSuccess => Succeeded;
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Result Success(string? message = null)
    {
        return new Result { Succeeded = true, Message = message };
    }

    public static Result Failure(params string[] errors)
    {
        return new Result { Succeeded = false, Errors = errors.ToList() };
    }
}

/// <summary>
/// Kết quả trả về có dữ liệu
/// </summary>
public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data, string? message = null)
    {
        return new Result<T> { Succeeded = true, Data = data, Message = message };
    }

    public new static Result<T> Failure(params string[] errors)
    {
        return new Result<T> { Succeeded = false, Errors = errors.ToList() };
    }
}