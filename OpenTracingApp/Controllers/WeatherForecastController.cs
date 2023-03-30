using Microsoft.AspNetCore.Mvc;
using OpenTracing;

namespace OpenTracingApp.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ITracer _tracer;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ITracer tracer)
    {
        _logger = logger;
        _tracer = tracer;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        var currentScope = HttpContext.Items["CurrentTracingScope"] as IScope;
        using var span = _tracer.BuildSpan("MySpan").AsChildOf(currentScope?.Span).StartActive();
        
        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        
        span?.Span.SetTag("forecastsCount", forecasts.Length);
        
        return forecasts;
    }
}