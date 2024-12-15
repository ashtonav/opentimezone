using System.Runtime.InteropServices;
using TimezoneWebApi.Dependency;

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    throw new PlatformNotSupportedException("Currently, this application is designed to work exclusively on Linux/Docker due to its specific timezone support. Support for other operating systems is not yet provided.");
}

var builder = WebApplication.CreateBuilder(args);

var app = builder
    .ConfigureServices()
    .AddOpenTelemetryLogs()
    .Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0&tabs=visual-studio#add-and-configure-swagger-middleware
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection()
    .AllowAllCors()
    .UseRouting()
    .UseExceptionHandler("/error") // When an error happens, use ErrorController.cs
    .UseEndpoints(endpoints => endpoints.MapControllers());

await app.RunAsync();
