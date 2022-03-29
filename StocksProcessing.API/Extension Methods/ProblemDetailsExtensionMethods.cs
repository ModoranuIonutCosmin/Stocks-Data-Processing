using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace StocksProcessing.API.Middleware;

public static class ProblemDetailsExtensionMethods
{
    public const string EXCEPTION_SUFFIX = "Exception";

    public static ProblemDetails MapToProblemDetailsWithStatusCode(this Exception details,
        HttpStatusCode statusCode)
    {
        var exceptionTitle = details.GetType().Name;

        if (exceptionTitle.EndsWith(EXCEPTION_SUFFIX))
            exceptionTitle = exceptionTitle[..exceptionTitle.LastIndexOf(EXCEPTION_SUFFIX)];

        return new ProblemDetails
        {
            Detail = details.Message,
            Status = (int) statusCode,
            Title = exceptionTitle,
            Type = $"https://httpstatuses.com/{(int) statusCode}"
        };
    }
}