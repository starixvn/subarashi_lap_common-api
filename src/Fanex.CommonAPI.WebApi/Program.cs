using Fanex.CommonAPI.Presentation.Extensions;
using Fanex.CommonAPI.WebApi.Startup;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddDependencyInjection(builder.Configuration);

var app = builder.Build();

app.ConfigureHealthCheck();
app.UseForwardedHeaders();

app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/api"),
    config => config.UseHttpLogging());

app.ConfigureExceptionHandler(builder.Configuration);

app.ConfigureSwagger(builder.Configuration);

app.UseRouting();

app.UseCors();

app.UseRequestLocalization();

app
    .MapGet("/", () => "Common API is running ...")
    .ExcludeFromDescription();

app.MapEndpoints();

app.Run();