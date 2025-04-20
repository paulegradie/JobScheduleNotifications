using JobScheduleNotifications.Composition;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Quick & decent logging setup
builder.Logging.ClearProviders(); // Optional: Clears default providers
builder.Logging.AddConsole();     // Adds basic console logging
builder.Logging.AddDebug();       // Useful in Visual Studio output window

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Compose application dependencies
builder.Services.ComposeApplication(builder.Configuration);

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

// 🔹 Log the addresses after the app starts
app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
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

app.Run();