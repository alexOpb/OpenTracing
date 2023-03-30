using OpenTracing;

namespace OpenTracingApp;

public class OpenTracingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITracer _tracer;

    public OpenTracingMiddleware(RequestDelegate next, ITracer tracer)
    {
        _next = next;
        _tracer = tracer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var name = $"{context.Request.Method} {context.Request.Path}";
        using var scope = _tracer.BuildSpan(name).StartActive();
        context.Items["CurrentTracingScope"] = scope;
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            scope.Span.SetTag("error", true);
            scope.Span.Log(new Dictionary<string, object>
            {
                { LogFields.Event, "error" },
                { LogFields.ErrorKind, ex.GetType().Name },
                { LogFields.ErrorObject, ex }
            });
            throw;
        }
    }
}