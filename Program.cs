using Microsoft.EntityFrameworkCore;
using EP.API.Data;
using EP.API.Mapping;
using EP.API.Middleware;  
using EP.API.Repositories;
using EP.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = false;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "EP - Event Platform API",
        Version = "v1",
        Description = "Backend API for organizing and scheduling events. Manages events, categories, venues, speakers, and timelines."
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("EP.API")));

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<ISpeakerService, SpeakerService>();

builder.Services.AddHealthChecks();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
        else
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");

app.MapHealthChecks("/health");
app.MapControllers();

await InitializeDatabaseAsync(app.Services, app.Environment);

app.Run();

static async Task InitializeDatabaseAsync(IServiceProvider services, IHostEnvironment environment)
{
    using var scope = services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("DatabaseInitializer");

    try
    {
        var migrations = db.Database.GetMigrations();
        if (migrations.Any())
        {
            await db.Database.MigrateAsync();
            logger.LogInformation("Database migrated successfully.");
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
            logger.LogInformation("Database schema ensured successfully.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed.");

        if (!environment.IsDevelopment())
        {
            throw;
        }
    }
}
