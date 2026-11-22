using System.Security.Claims;
using TasinmazYonetimi.Constants;
using TasinmazYonetimi.Interfaces;

namespace TasinmazYonetimi.Middlewares
{
    // Yakalanmamış hataları loglar ve 500 döner
    public class HataYakalamaMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HataYakalamaMiddleware> _logger;

        public HataYakalamaMiddleware(
            RequestDelegate next,
            ILogger<HataYakalamaMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IServiceScopeFactory scopeFactory)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Beklenmeyen hata");

                try
                {
                    // Bozulmuş olabilecek DbContext yerine yeni scope
                    using var scope = scopeFactory.CreateScope();
                    var logService = scope.ServiceProvider
                        .GetRequiredService<ILogService>();

                    int? kullaniciId = int.TryParse(
                        context.User.FindFirstValue(ClaimTypes.NameIdentifier),
                        out var id) ? id : null;

                    await logService.AddLogAsync(
                        kullaniciId,
                        IslemTipleriSabit.Hata,
                        DurumlarSabit.Basarisiz,
                        $"{context.Request.Method} {context.Request.Path}: {ex.Message}",
                        context.Connection.RemoteIpAddress?.ToString());
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Hata logu yazılamadı.");
                }

                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Beklenmeyen bir hata oluştu."
                });
            }
        }
    }
}
