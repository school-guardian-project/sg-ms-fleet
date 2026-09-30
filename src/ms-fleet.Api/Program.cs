using ms_fleet.Api.Infrastructure.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddFleetServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
