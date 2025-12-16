using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ApiGateway.Extensions;
using Prometheus;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    $"Configuration/ocelot.{builder.Environment.EnvironmentName}.json",
    optional: false,
    reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 添加 OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource("ApiGateway")
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("ApiGateway"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddZipkinExporter(options =>
            {
                var zipkinUrl = builder.Configuration["Zipkin:Endpoint"]
                    ?? "http://zipkin:9411/api/v2/spans";
                options.Endpoint = new Uri(zipkinUrl);
            });
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAll");

// Prometheus 指标
app.UseMetricServer();
app.UseHttpMetrics();


app.UseRequestLogging();

app.MapHealthChecks("/health");

await app.UseOcelot();

app.Run();