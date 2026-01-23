using System.IO;
using Journal.Data;
using Journal.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Journal
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

            // SQLite database stored in app private folder
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal.db");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Filename={dbPath}"));

            // DbContext is scoped -> service should also be scoped
            builder.Services.AddScoped<JournalService>();

            // Auth can stay singleton if it doesn't depend on DbContext
            builder.Services.AddSingleton<AuthService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
