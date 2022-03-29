using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace StocksProcessing.API.Payloads;

public static class ApiResponseExtensionsHelpers
{
    public static ApiResponse<T> FailedApiOperationResponse<T>(this ControllerBase controller,
        string reason,
        T value = default,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var result = new ApiResponse<T>();

        controller.Response.StatusCode = (int) statusCode;
        result.ErrorMessage = reason;
        result.Response = value;

        return result;
    }
}