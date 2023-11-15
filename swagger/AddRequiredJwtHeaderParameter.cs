using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

class AddRequiredJwtHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        if (context.MethodInfo.GetCustomAttributes(false).Any(a => a is JwtAuthAttribute))
        {
            operation.Parameters.Add(
                new()
                {
                    Name = "X-Authorization",
                    In = ParameterLocation.Header,
                    Required = true,
                    Description = "JWT Authentication Bearer token"
                }
            );
        }
    }
}
