
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Serilog;
using VITRACK.Api.Extensions;
using VITRACK.Common.Helpers;
using VITRACK.Infrastructure.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "ViTrackAPI",
        Version = "v1"
    });

});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ===== Extensions =====
// Add Idenity Configuration
builder.Services.AddApplicationIdentity();
// Register application repositories
builder.Services.AddApplicationRepositories();
// Add JWT Authentication
var secretKey = builder.Configuration.GetSection("AppSettings:jwt_secret_key").Value;
if (string.IsNullOrEmpty(secretKey))
    throw new Exception("Jwt Secret Key not found!");
builder.Services.AddJwtAuthentication(secretKey);
// Add CORS Policy
builder.Services.AddAllowedSpecificOrigins();
// ===== Extensions =====

// ===== Log configuration =====
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
// ===== Log configuration =====

var app = builder.Build();

app.UseSerilogRequestLogging();
Log.Warning("App is started at " + TimeHelper.GetBakuTime().ToString());

// Reccomend to disbale after finishing development
await app.ApplyMigrationsAsync();
await app.AppendSeedRolesAsync();

// pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ViTrackAPI v1");
    });
}
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();


// Enable serving static files (for image uploads)
app.UseStaticFiles();
app.UseDefaultFiles();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(
        app.Environment.WebRootPath, @"uploads")),
    RequestPath = new PathString("/uploads")
});

app.UseHttpsRedirection();
app.MapControllers();

app.MapFallbackToFile("index.html");
Log.Warning("App is runned at " + TimeHelper.GetBakuTime().ToString());

app.Run();



