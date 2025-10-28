using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using BasicBackend.Infrastructure.Data;
using BasicBackend.Infrastructure.Repositories;
using BasicBackend.Application.Services;
using BasicBackend.Api;
using BasicBackend.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDb>(opt => opt.UseInMemoryDatabase("app"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter("fixed", o =>
{
    o.PermitLimit = 100;
    o.Window = TimeSpan.FromMinutes(1);
    o.QueueLimit = 50;
}));

var app = builder.Build();

app.UseExceptionHandler(a => a.Run(async ctx =>
{
    ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(new { error = "Internal Server Error", traceId = ctx.TraceIdentifier });
}));

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRateLimiter();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapUsersEndpoints();

app.Run();
