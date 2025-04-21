using System.Text;
using Api.Composition;
using Api.Infrastructure.Auth;
using Api.Infrastructure.Data;
using Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders(); // Optional: Clears default providers
builder.Logging.AddConsole();     // Adds basic console logging
builder.Logging.AddDebug();       // Useful in Visual Studio output window
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
// Compose application dependencies
builder.Services.ComposeApplication(builder.Configuration);
builder.Services.ConfigureAuthentication();

var app = builder.Build();
app.Services.EnsureAndMigrateDatabase();
await app.EnsureDefaultRoles();

app.UseMiddleware<ErrorHandlingMiddleware>();

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

app.Run();


namespace Api
{
    public partial class Program
    {
    }
}