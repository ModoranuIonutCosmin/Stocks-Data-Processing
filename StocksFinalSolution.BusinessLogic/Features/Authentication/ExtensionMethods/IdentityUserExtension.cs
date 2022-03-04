using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StocksProcessing.API.Auth
{
    /// <summary>
    /// Extension methods for <see cref="IdentityError"/> class
    /// </summary>
    public static class IdentityUserExtension
    {
        /// <summary>
        /// Combines all errors into a single string
        /// </summary>
        /// <param name="errors">The errors to aggregate</param>
        /// <returns>Returns a string with each error separated by a new line</returns>
        public static string AggregateErrors(this IEnumerable<IdentityError> errors)
        {
            // Get all errors into a list
            return errors?.Select(f => f.Description)
                          // And combine them with a newline separator
                          .Aggregate((a, b) => $"{a}{Environment.NewLine}{b}");
        }
    }
}
