using System.Reflection;
using Microsoft.OpenApi;

namespace TasinmazYonetimi.Extensions
{
    public static class ServiceExtensions
    {
        public const string FrontendCorsPolicy = "FrontendCorsPolicy";
        private const string BearerSchemeName = "Bearer";

        // Swagger UI ve JWT yetkilendirme yapılandırması
        public static IServiceCollection AddSwaggerDocumentation(
            this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Taşınmaz Yönetimi API",
                    Version = "v1",
                    Description =
                        "Taşınmaz, kullanıcı ve log işlemleri için REST API"
                });

                // JWT güvenlik tanımı
                options.AddSecurityDefinition(
                    BearerSchemeName,
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "Giriş sonrası alınan token'ı 'Bearer' yazmadan giriniz."
                    });

                // Tüm endpoint'lerde token kullan
                options.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(
                            BearerSchemeName, document)] = []
                    });

                // Controller'lardaki /// yorumlarını Swagger'da göster
                var xmlDosyasi =
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                var xmlYolu = Path.Combine(
                    AppContext.BaseDirectory, xmlDosyasi);

                if (File.Exists(xmlYolu))
                    options.IncludeXmlComments(xmlYolu);
            });

            return services;
        }

        // Angular uygulamasının API'ye erişebilmesi için CORS
        public static IServiceCollection AddFrontendCors(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var izinliAdresler = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>();

            if (izinliAdresler == null || izinliAdresler.Length == 0)
                throw new InvalidOperationException(
                    "CORS için izin verilen adres bulunamadı.");

            services.AddCors(options =>
            {
                options.AddPolicy(FrontendCorsPolicy, policy =>
                {
                    policy
                        .WithOrigins(izinliAdresler)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        // Excel indirmede dosya adını okuyabilmek için
                        .WithExposedHeaders("Content-Disposition");
                });
            });

            return services;
        }
    }
}
