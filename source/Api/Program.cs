using Api.Composition;
using Api.Infrastructure.Auth;
using Api.Infrastructure.Data;
using Api.Infrastructure.EntityFramework;
using Api.Infrastructure.Identity;
using Api.Middleware;
using Api.ModelBinders;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

var builder = WebApplication.CreateBuilder(args);

var url = builder.Configuration.GetSection("BaseUrl").Value;
builder.WebHost.UseUrls();

builder.Logging.ClearProviders(); // Optional: Clears default providers
builder.Logging.AddConsole(); // Adds basic console logging
builder.Logging.AddDebug(); // Useful in Visual Studio output window
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders
        .Insert(0, new GuidBackedValueObjectModelBinderProvider());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseDefaultServiceProvider(sp => { sp.ValidateScopes = true; });
builder.Services.ComposeApplication(builder.Configuration);
builder.Services.AddConfiguredDbContextAndConventions(builder.Configuration, builder.Environment);
builder.Services.AddIdentityServices();
builder.Services.AddRolePolicies();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP on 5000 (optional if you still want it)
    options.ListenLocalhost(5000);

    // HTTPS on 5001 using your dev cert
    options.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.UseHttps(); 
    });
});

var app = builder.Build();


app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<CurrentUserMiddleware>();
app.UseAuthorization();
app.MapControllers();

// Log the addresses after the app starts
app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Api.Program>>();
    var serverAddresses = app.Services.GetService<IServer>()?
        .Features.Get<IServerAddressesFeature>();

    if (serverAddresses != null)
    {
        foreach (var address in serverAddresses.Addresses)
        {
            logger.LogInformation("Server is listening on: {Address}", address);
        }
    }
    else
    {
        logger.LogWarning("Could not retrieve server addresses. This might happen when using Kestrel directly without addresses specified.");
    }
});

if (!app.Environment.IsEnvironment("Test"))
{
    await app.Services.EnsureAndMigrateDatabase(preDelete: false);
}

await app.Services.SeedRolesAsync();

app.Run();


namespace Api
{
    public partial class Program
    {
    }
}