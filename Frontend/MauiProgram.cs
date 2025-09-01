using Frontend.Handler;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Frontend.Services;
using Blazored.Toast;

namespace Frontend
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddBlazoredToast();

            builder.Services.AddTransient<AuthHeaderHandler>();
            builder.Services.AddHttpClient("WebAPI", client =>
            {
                // Set the base URL for your API
                client.BaseAddress = new Uri("http://localhost:8000");
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<CommunityService>();
            builder.Services.AddScoped<PostService>();

            return builder.Build();
        }
    }
}
