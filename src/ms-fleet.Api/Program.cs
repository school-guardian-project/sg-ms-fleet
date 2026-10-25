using ms_fleet.Api.Infrastructure.Auth;
using ms_fleet.Api.Infrastructure.DependencyInjection;
using ms_fleet.Api.Infrastructure.Grpc;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Techo global (no depende de IP ni de usuario): acota el trabajo simultaneo
    // y la rafaga para que un flood desde una sola IP, o con IP falsificada via
    // cabeceras, nunca sature el backend. La concurrencia va primero para que un
    // rechazo de la ventana no consuma cupo ya adquirido.
    options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
        PartitionedRateLimiter.Create<HttpContext, string>(_ =>
            RateLimitPartition.GetConcurrencyLimiter("global", _ => new ConcurrencyLimiterOptions
            {
                PermitLimit = 40, // ~ Max Pool Size (conexiones simultaneas a BD)
                QueueLimit = 0
            })),
        PartitionedRateLimiter.Create<HttpContext, string>(_ =>
            RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromSeconds(10),
                QueueLimit = 0
            })));

    // Escrituras (alta/edicion/borrado): 30/min por IP para frenar abuso.
    options.AddPolicy("writes", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 30,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

builder.Services.AddFleetServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRateLimiter();
app.MapControllers();

// Servidor interno route.v1.RouteService (consultas de bus de ms-route).
// Solo responde sobre el endpoint h2c dedicado que declara .env.example.
app.MapGrpcService<RouteGrpcService>();

app.Run();
