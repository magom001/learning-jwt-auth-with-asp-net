static class JwtAuthenticationMiddleware
{
    public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder app)
    {
        app.Use(
            async (context, next) =>
            {
                context.Request.Headers.TryGetValue("x-authorization", out var value);
                Console.WriteLine(
                    $"This is invoked before the controller. Authorization header value is {value}"
                );
                await next.Invoke();
                Console.WriteLine("And this is invoked after the controller's body");
            }
        );

        return app;
    }
}
