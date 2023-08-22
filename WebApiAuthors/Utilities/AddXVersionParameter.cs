using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiAuthors.Utilities
{
    public class AddXVersionParameter : IOperationFilter
    {
        /// <summary>
        /// Method to filter and add x-version parameter to all the controllers
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "x-version",
                In = ParameterLocation.Header,
                Required = true
            });
        }
    }
}
