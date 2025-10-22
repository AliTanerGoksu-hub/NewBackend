using BarcodeScanning;
using BusinessSmartMobile.Services;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace BusinessSmartMobile
{
    public static class MauiProgram
    {
        public static MauiApp CurrentApp { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseBarcodeScanning()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();

            // Ayarlar
            builder.Services.AddSingleton<SettingsService>();

            // HttpClient – SettingsService'ten BaseAddress alıyor
            builder.Services.AddScoped(sp =>
            {
                var apiSettings = sp.GetRequiredService<SettingsService>();
                return new HttpClient { BaseAddress = new Uri(apiSettings.GetApiBaseUrl()) };
            });

            // Servisler
            builder.Services.AddScoped<BusinessPartnerService>();
            builder.Services.AddScoped<StockService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<ReportsService>();
            builder.Services.AddScoped<PaymentService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<DeviceService>();

            // Sepeti tüm uygulama boyunca korumak istiyorsan Singleton mantıklı:
            builder.Services.AddSingleton<CatalogService>();
            // Eğer her Blazor “circuit” için izole olsun dersen: AddScoped<CatalogService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();   // <-- TEK KEZ build
            CurrentApp = app;
            return app;                  // <-- Aynı instance’ı döndür
        }
    }
}
