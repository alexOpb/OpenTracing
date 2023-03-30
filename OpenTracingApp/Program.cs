using Jaeger;
using Jaeger.Samplers;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using OpenTracing;
using OpenTracingApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITracer>(
    sp =>
    {
        var loggerFactory = sp.GetService<ILoggerFactory>();
        Configuration.SenderConfiguration.DefaultSenderResolver = new SenderResolver(loggerFactory)
            .RegisterSenderFactory<ThriftSenderFactory>();
        var tracer = new Tracer.Builder("open-tracing-service")
            .WithLoggerFactory(loggerFactory)
            .WithSampler(new ProbabilisticSampler(0.1))
            .Build();
        return tracer;
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<OpenTracingMiddleware>();

app.Run();