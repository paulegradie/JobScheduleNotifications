using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Server.Contracts.Endpoints;

public class OperationResult<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess { get; }

    public T? Value { get; }              // still declared nullable
    public string? ErrorMessage { get; }
    public HttpStatusCode StatusCode { get; }

    private OperationResult(bool isSuccess, T? value, string? errorMessage, HttpStatusCode statusCode)
    {
        IsSuccess     = isSuccess;
        Value         = value;
        ErrorMessage  = errorMessage;
        StatusCode    = statusCode;
    }

    public static OperationResult<T> Success(T value, HttpStatusCode statusCode) =>
        new OperationResult<T>(true,  value, null, statusCode);

    public static OperationResult<T> Failure(string errorMessage, HttpStatusCode statusCode) =>
        new OperationResult<T>(false, default, errorMessage, statusCode);
}

public class OperationResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public HttpStatusCode StatusCode { get; }

    private OperationResult(bool isSuccess, string? errorMessage, HttpStatusCode statusCode)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public static OperationResult Success(HttpStatusCode statusCode) =>
        new OperationResult(true, null, statusCode);

    public static OperationResult Failure(string errorMessage, HttpStatusCode statusCode) =>
        new OperationResult(false, errorMessage, statusCode);
}