class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate next;

    public JwtAuthenticationMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtAuthenticationService authService)
    {
        context.Request.Headers.TryGetValue("x-authorization", out var authHeaders);
        var authHeader = authHeaders.FirstOrDefault<string>("");

        if (string.IsNullOrEmpty(authHeader))
        {
            await this.next(context);
            return;
        }

        var token = authHeader.Split(" ").Last();

        if (token is null)
        {
            await this.next(context);
            return;
        }

        var jwtTokenPayload = authService.DecryptJwtToken(token);

        context.Items.Add("x-current-user", jwtTokenPayload);
        await this.next(context);
    }
}
