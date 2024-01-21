

using System.Text.Json;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtensions

    {
        /// <summary>
        /// Adds pagination information to the HTTP response headers.
        /// </summary>
        /// <param name="response">The HTTP response to which pagination headers are added.</param>
        /// <param name="header">The pagination header containing relevant information.</param>
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            // Configure JSON options for serialization
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            // Add pagination information to the response headers
            response.Headers.Append("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            // Expose the Pagination header for client-side access
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        }
    }
}