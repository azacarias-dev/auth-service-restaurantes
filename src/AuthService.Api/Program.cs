using AuthService.Api.Extensions;
using AuthService.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------
// SERVICIOS
// -------------------------------------

builder.Services.AddControllers();

// 🔥 Swagger limpio (SIN duplicar)
builder.Services.AddApiDocumentation();

// Servicios
builder.Services.AddApplicationServices(builder.Configuration);

// Fix PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

// -------------------------------------
// MIDDLEWARES
// -------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// -------------------------------------
// DB INIT
// -------------------------------------

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Iniciando la migracion de la base de datos");

        await DataSeeder.SeedAsync(context);

        logger.LogInformation("Datos iniciales sembrados exitosamente");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al inicializar la base de datos");
        throw;
    }
}

app.Run();