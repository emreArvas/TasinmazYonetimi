
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using TasinmazYonetimi.Data;
using TasinmazYonetimi.Interfaces;
using TasinmazYonetimi.Mappings;
using TasinmazYonetimi.Services;
using Microsoft.AspNetCore.Identity;
using TasinmazYonetimi.Models;
using TasinmazYonetimi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// veritabanı bağlantısı
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Veritabanı bağlantı bilgisi bulunamadı.");

builder.Services.AddDbContext<TasinmazYonetimiContext>(
    options => options.UseNpgsql(connectionString));

builder.Services.AddControllers();

// Swagger UI
builder.Services.AddSwaggerDocumentation();

// Angular için CORS
builder.Services.AddFrontendCors(builder.Configuration);

// Auto mapper servisi
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(MappingProfile));

// Uygulama servisleri
//"Bir yerde IIlService istenirse IlService sınıfını kullan."
builder.Services.AddScoped<IIlService, IlService>();
builder.Services.AddScoped<IIlceService, IlceService>();
builder.Services.AddScoped<IMahalleService, MahalleService>();
builder.Services.AddScoped<ITasinmazService, TasinmazService>();
builder.Services.AddScoped<IKullaniciService, KullaniciService>();

builder.Services.AddScoped<
    IPasswordHasher<Kullanicilar>,
    PasswordHasher<Kullanicilar>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILogService, LogService>();



// JWT yapılandırması
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT anahtarı bulunamadı.");

var jwtKeyBytes = Convert.FromBase64String(jwtKey);

if (jwtKeyBytes.Length < 32)
    throw new InvalidOperationException(
        "JWT anahtarı en az 32 bayt olmalıdır.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT Issuer bulunamadı.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT Audience bulunamadı.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireSignedTokens = true,
            RequireExpirationTime = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),

            NameClaimType = System.Security.Claims.ClaimTypes.Email,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,

            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Taşınmaz Yönetimi API v1");

        options.DocumentTitle = "Taşınmaz Yönetimi API";

        // Sayfa yenilenince token silinmesin
        options.EnablePersistAuthorization();
    });
}

app.UseHttpsRedirection();

// Sıra önemli: CORS, kimlik doğrulamadan önce gelmeli
app.UseCors(ServiceExtensions.FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
