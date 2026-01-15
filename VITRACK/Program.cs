
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

// ===== Add Idenity Configuration =====
builder.Services.AddApplicationIdentity();
// ===== Add Idenity Configuration =====

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


app.Run();

