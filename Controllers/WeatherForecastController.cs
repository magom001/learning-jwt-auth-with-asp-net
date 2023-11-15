using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Mvc;

namespace jwt_knowledge_sharing.Controllers;

[ApiController]
[Route("api")]
public class WeatherForecastController : ControllerBase
{
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

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
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
