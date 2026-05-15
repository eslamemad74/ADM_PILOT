using Microsoft.EntityFrameworkCore;
using AdmWorksheet.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    }); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ADM Worksheet API",
        Version = "v1",
        Description = "Aviation Decision-Making Worksheet — IMSAFE, PAVE, DECIDE"
    });
});

// SQLite — file-based, zero-install, works on instructor's machine without SQL Server
var dbPath = Path.Combine(AppContext.BaseDirectory, "adm_worksheet.db");
builder.Services.AddDbContext<AdmDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Allow serving static frontend files
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Auto-create database on startup (no migration commands needed)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AdmDbContext>();
    db.Database.EnsureCreated();
}

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ADM Worksheet API v1");
    c.RoutePrefix = "swagger";
});

// Serve static frontend
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.MapControllers();

// Fallback to index.html for SPA routing
app.MapFallbackToFile("index.html");
app.Urls.Add("http://localhost:5200");
app.Run();