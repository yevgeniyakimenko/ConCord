using ConCord.Models;
using ConCord.Controllers;
using ConCord.Hubs;
using Microsoft.EntityFrameworkCore;
using Npgsql;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var dbString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

builder.Services.AddDbContext<DatabaseContext>(
    opt => {
        opt.UseNpgsql(dbString, options =>
        {
            options.EnableRetryOnFailure();
        });
        if (builder.Environment.IsDevelopment())
        {
            opt
                .LogTo(Console.WriteLine, LogLevel.Warning)
                .EnableDetailedErrors();
        }
    }
);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8081"; //
builder.WebHost.UseUrls($"http://0.0.0.0:{port}"); //

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<ChatHub>("/r/chatHub");

app.UseDefaultFiles(); //
app.UseStaticFiles(); //
app.MapFallbackToFile("index.html"); //

app.Run();
