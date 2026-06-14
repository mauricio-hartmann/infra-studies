using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace IS.Core.API.Results
{
    public class BadRequestProblemDetails : ProblemDetails
    {
        [JsonPropertyName("errors")]
        public IDictionary<string, ICollection<string>> Errors { get; }

        public BadRequestProblemDetails(IDictionary<string, ICollection<string>> errors, HttpContext httpContext)
        {
            Type = "https://httpstatuses.com/400";
            Title = "Invalid request data.";
            Status = StatusCodes.Status400BadRequest;
            Detail = "One or more business validation errors occurred.";
            Instance = httpContext.Request.Path;
            Errors = errors;
            Extensions["traceId"] = httpContext.TraceIdentifier;
        }
    }
}
