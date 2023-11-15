using Microsoft.AspNetCore.Http.Features;

static class JwtAuthorizationMiddleware
{
    public static IApplicationBuilder UseJwtAuthorization(this IApplicationBuilder app)
    {
        app.Use(
            async (context, next) =>
            {
                // Let's see if our endpoint is annotated with our custom JwtAuthAttribute
                var isAnnotatedWithJwtAuthAttribute =
                    context.Features
                        .Get<IEndpointFeature>()
                        ?.Endpoint?.Metadata.Any(m => m is JwtAuthAttribute) ?? false;

                if (isAnnotatedWithJwtAuthAttribute)
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                Console.WriteLine(
                    "This is invoked before the controller and after the JwtAuthenticationMiddleware"
                );
                await next.Invoke();
                Console.WriteLine("And this is invoked after the controller's body");
            }
        );

        return app;
    }
}
