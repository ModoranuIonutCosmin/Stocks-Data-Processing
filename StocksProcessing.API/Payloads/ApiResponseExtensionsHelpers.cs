﻿using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace StocksProcessing.API.Payloads
{
    public static class ApiResponseExtensionsHelpers
    {
        public static ApiResponse<T> FailedApiOperationResponse<T>(this ControllerBase controller,
            string reason,
            T value = default,
            HttpStatusCode statusCode = HttpStatusCode.NotFound)
        {
            var result = new ApiResponse<T>();

            controller.Response.StatusCode = (int)statusCode;
            result.ErrorMessage = reason;
            result.Response = value;

            return result;
        }
    }
}
