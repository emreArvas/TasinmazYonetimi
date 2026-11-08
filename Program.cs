
using Microsoft.EntityFrameworkCore;
using TasinmazYonetimi.Data;
using TasinmazYonetimi.Interfaces;
using TasinmazYonetimi.Mappings;
using TasinmazYonetimi.Services;

var builder = WebApplication.CreateBuilder(args);

// veritabanı bağlantısı
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Veritabanı bağlantı bilgisi bulunamadı.");

builder.Services.AddDbContext<TasinmazYonetimiContext>(
    options => options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Auto mapper servisi
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(MappingProfile));

// Uygulama servisleri
builder.Services.AddScoped<IIlService, IlService>();
builder.Services.AddScoped<IIlceService, IlceService>();
builder.Services.AddScoped<IMahalleService, MahalleService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
