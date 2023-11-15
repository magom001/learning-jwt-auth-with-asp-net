using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Mvc;

namespace jwt_knowledge_sharing.Controllers;

[ApiController]
[Route("api")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> logger;
    private readonly IJwtAuthenticationService jwtAuthService;

    public WeatherForecastController(
        IJwtAuthenticationService jwtAuthService,
        ILogger<WeatherForecastController> logger
    )
    {
        this.logger = logger;
        this.jwtAuthService = jwtAuthService;
    }

    private static readonly string[] Summaries = new[]
    {
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    };

    [HttpPost("login")]
    public IActionResult Login(LoginDto loginDto)
    {
        var token = this.jwtAuthService.AuthenticateByUsernameAndPassword(
            loginDto.username,
            loginDto.password
        );

        if (token is null)
        {
            return this.Unauthorized();
        }

        return this.Ok(new AccessTokenDto(token));
    }

    [JwtAuth]
    [HttpGet("protected")]
    public OkObjectResult ProtectedGet()
    {
        return this.Ok("you are getting a protected resource");
    }

    [HttpGet("anonymous")]
    public IEnumerable<WeatherForecast> Get()
    {
        Console.WriteLine("Controller is called");
        return Enumerable
            .Range(1, 5)
            .Select(
                index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    }
            )
            .ToArray();
    }
}
