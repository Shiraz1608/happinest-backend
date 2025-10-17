using Happinest.GatewaySolution.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add authentication extension
builder.AddAppAuthetication();
// Set content root and load configuration
builder.Host.UseContentRoot(Directory.GetCurrentDirectory());

// Load Ocelot configuration based on environment
if (builder.Environment.EnvironmentName.ToLower() == "production")
{
    builder.Configuration.AddJsonFile("ocelot.Production.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
}

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Serve static files (wwwroot/index.html by default)
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});
app.UseStaticFiles();


// Add Ocelot middleware for API Gateway
app.UseOcelot().GetAwaiter().GetResult();

app.Run();
