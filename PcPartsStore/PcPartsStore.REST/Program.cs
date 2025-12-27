using Microsoft.EntityFrameworkCore;
using PcPartsStore.Infrastructure;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON (щоб не падало на циклах у навігаційних властивостях EF)
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// EF Core (SQLite)
builder.Services.AddDbContext<PcPartsStoreContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("PcPartsStoreDb");
    options.UseSqlite(cs);
});

// Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Дуже часто саме це фіксить 500 на swagger.json (конфлікт назв схем)
    c.CustomSchemaIds(t => t.FullName);
});

var app = builder.Build();

// Вивід детальних помилок у Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Автоматично застосувати міграції
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PcPartsStoreContext>();
    db.Database.Migrate();
}

// !!! Вимикаємо, бо ти запускаєш лише HTTP (і є warning про https port)
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
