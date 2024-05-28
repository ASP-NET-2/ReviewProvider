using Infrastructure.Models.ResultModels;
using Microsoft.AspNetCore.Mvc;

namespace ResetPassword.Factories;

public static class ObjectResultFactory
{
    /// <summary>
    /// Creates an ObjectResult from a process result. The message will be assigned as the value in plain text.
    /// </summary>
    public static ObjectResult CreateFromProcessResult(ProcessResult processResult)
    {
        return new ObjectResult(processResult.Message) { StatusCode = processResult.StatusCode };
    }

    /// <summary>
    /// Creates an ObjectResult from a process result. If processResult.ResultObject has a non-null
    /// value, it will be provided as value in the ObjectResult, otherwise the message will.
    /// </summary>
    public static ObjectResult CreateFromProcessResult<T>(ProcessResult<T> processResult)
    {
        return new ObjectResult(processResult.ResultObject != null 
            ? processResult.ResultObject 
            : processResult.Message) { StatusCode = processResult.StatusCode };
    }
}
