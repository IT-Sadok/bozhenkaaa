namespace Experiments.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string Error { get; }

    public string? Source { get; }

    private Result(bool isSuccess, string error, string? source = null)
    {
        switch (isSuccess)
        {
            case true when error != string.Empty:
                throw new InvalidOperationException("Successful result cannot have an error.");
            case false when error == string.Empty:
                throw new InvalidOperationException("Failure result must have an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
        Source = source;
    }

    public static Result Success() => new(true, string.Empty);

    public static Result Failure(string message, string? source = null) => new(false, message, source);

    public static Result Failure(IEnumerable<string> errors, string? source = null) =>
        Failure(string.Join("; ", errors), source);
}

public class Result<T>
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T? Value { get; }

    public string Error { get; }

    public string? Source { get; }

    private Result(bool isSuccess, T? value, string error, string? source = null)
    {
        switch (isSuccess)
        {
            case true when error != string.Empty:
                throw new InvalidOperationException("Successful result cannot have an error.");
            case false when error == string.Empty:
                throw new InvalidOperationException("Failure result must have an error.");
        }

        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Source = source;
    }

    public static Result<T> Success(T value) => new(true, value, string.Empty);

    public static Result<T> Failure(string message, string? source = null) =>
        new(false, default, message, source);

    public static Result<T> Failure(IEnumerable<string> errors, string? source = null) =>
        Failure(string.Join("; ", errors), source);
}
