
using Microsoft.EntityFrameworkCore;
using VITRACK.Api.Extensions;
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
// ===== Extensions =====


var app = builder.Build();

// pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ViTrackAPI v1");
    });
    await app.ApplyMigrationsAndSeedRolesAsync();
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseAuthentication();


app.Run();

