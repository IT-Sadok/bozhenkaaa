namespace AIAnalysis.Domain.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    
    public bool IsFailure => !IsSuccess;
    
    public T? Value { get; }
    
    public string Error { get; }

    private Result(bool isSuccess, T? value, string error)
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
    }

    public static Result<T> Success(T value) => new(true, value, string.Empty);
    
    public static Result<T> ErrorResult(string error) => new(false, default, error);
}