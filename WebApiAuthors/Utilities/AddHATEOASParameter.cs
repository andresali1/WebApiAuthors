using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiAuthors.Utilities
{
    public class AddHATEOASParameter : IOperationFilter
    {
        /// <summary>
        /// Method to catch the query string without writting it on each method
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if(context.ApiDescription.HttpMethod != "GET")
            {
                return;
            }

            if(operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "includeHATEOAS",
                In = ParameterLocation.Header,
                Required = false
            });
        }
    }
}
