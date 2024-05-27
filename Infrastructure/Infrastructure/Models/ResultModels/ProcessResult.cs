using Microsoft.AspNetCore.Http;

namespace Infrastructure.Models.ResultModels;

public class ProcessResult
{
    public static ProcessResult BadRequestResult(string message) => new(StatusCodes.Status400BadRequest, message);
    public static ProcessResult ForbiddenResult(string message) => new(StatusCodes.Status403Forbidden, message);
    public static ProcessResult NotFoundResult(string message) => new(StatusCodes.Status404NotFound, message);
    public static ProcessResult ConflictResult(string message) => new(StatusCodes.Status409Conflict, message);
    public static ProcessResult InternalServerErrorResult(string message) => new(StatusCodes.Status500InternalServerError, message);

    public int StatusCode { get; private set; }
    public string Message { get; set; }

    public bool IsSuccessful => StatusCode >= 200 && StatusCode <= 299;

    public ProcessResult(int statusCode, string message = "")
    {
        StatusCode = statusCode;
        Message = message;
    }

    public ProcessResult<T> ToGeneric<T>() => new ProcessResult<T>(this);
}

public class ProcessResult<T> : ProcessResult
{
    public T? ResultObject { get; set; }

    public ProcessResult(int statusCode, string message = "", T? resultObject = default) : base(statusCode, message)
    {
        ResultObject = resultObject;
    }

    public ProcessResult(ProcessResult result) : base(result.StatusCode, result.Message)
    {
        ResultObject = default;
    }
}
