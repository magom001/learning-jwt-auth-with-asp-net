using Microsoft.AspNetCore.Http.Features;

static class JwtAuthorizationMiddleware
{
    public static IApplicationBuilder UseJwtAuthorization(this IApplicationBuilder app)
    {
        app.Use(
            async (context, next) =>
            {
                // Let's see if our endpoint is annotated with our custom JwtAuthAttribute
                var jwtAuthAnnotation =
                    context.Features
                        .Get<IEndpointFeature>()
                        ?.Endpoint?.Metadata.FirstOrDefault(m => m is JwtAuthAttribute)
                    as JwtAuthAttribute;

                if (jwtAuthAnnotation is not null)
                {
                    var currentUserJwtPayload = context.Items["x-current-user"] as JwtTokenPayload;

                    if (currentUserJwtPayload is null)
                    {
                        await Fail(context);
                        return;
                    }

                    if (jwtAuthAnnotation.Roles?.Any() == true)
                    {
                        var currentUserRoles = currentUserJwtPayload
                            .AppMetadata
                            ?.Authorization
                            ?.Roles;

                        if (currentUserRoles is null || !currentUserRoles.Any())
                        {
                            await Fail(context);
                            return;
                        }

                        if (!jwtAuthAnnotation.Roles.Any(role => currentUserRoles.Contains(role)))
                        {
                            await Fail(context);
                            return;
                        }
                    }
                }

                await next.Invoke();
            }
        );

        return app;
    }

    private static async Task Fail(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
    }
}
