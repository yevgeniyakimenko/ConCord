using ConCord.Models;
using ConCord.Controllers;
using ConCord.Hubs;
using ConCord.Services;
using Microsoft.Extensions.ML;
using Microsoft.EntityFrameworkCore;
using Npgsql;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var dbString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")?.Trim('"', '\'');

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

var modelPath = ConCord.ML.ToxicDetector.ResolveModelPath(builder.Environment.ContentRootPath);
builder.Services.AddPredictionEnginePool<ConCord.ML.ToxicDetector.ModelInput, ConCord.ML.ToxicDetector.ModelOutput>()
    .FromFile(modelName: ConCord.ML.ToxicDetector.ModelName, filePath: modelPath, watchForChanges: false);
builder.Services.AddSingleton<IToxicLanguageDetector, ToxicLanguageDetector>();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8081"; //
builder.WebHost.UseUrls($"http://0.0.0.0:{port}"); //

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.Migrate();
}

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

// Warm up the ML model pool to eliminate cold-start latency on first user message
var detector = app.Services.GetService<IToxicLanguageDetector>();
detector?.CheckToxicity("warmup");

app.Run();
