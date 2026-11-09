using System.Diagnostics.CodeAnalysis;

namespace NordicEmber.Api.Common;

public enum ResultErrorType
{
    Validation,
    Conflict
}

public record ResultError(ResultErrorType Type, string Field, string Message);

public record Result<T>
{
    public T? Value { get; }
    public ResultError? Error { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    private Result(T? value, ResultError? error)
    {
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(ResultError error) => new(default, error);
}
