using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Bot.Controllers;
using Bot.Services;
using Bot.Configuration;

namespace Bot;

class Program
{
    public static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // Объект, отвечающий за постоянный жизненный цикл приложения
        var host = new HostBuilder()
            .ConfigureServices((hostContext, services) => ConfigureServices(services)) // Задаем конфигурацию
            .UseConsoleLifetime() // Позволяет поддерживать приложение активным в консоли
            .Build(); // Собираем

        Console.WriteLine("Сервис запущен");
        // Запускаем сервис
        await host.RunAsync();
        Console.WriteLine("Сервис остановлен");
    }

    static void ConfigureServices(IServiceCollection services)
    {

        AppSettings appSettings = BuildAppSettings();
        services.AddSingleton(appSettings);

        services.AddSingleton<IStorage, MemoryStorage>();

        services.AddSingleton<IFileHandler, AudioFileHandler>();

        // Подключаем контроллеры сообщений и кнопок
        services.AddTransient<DefaultMessageController>();
        services.AddTransient<VoiceMessageController>();
        services.AddTransient<TextMessageController>();
        services.AddTransient<InlineKeyboardController>();

        services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(""));
        services.AddHostedService<Bot>();
    }

    static AppSettings BuildAppSettings()
    {
        return new AppSettings()
        {
            DownloadsFolder = "/home/user/",
            BotToken = "7866336182:AAGMkvbMZSk_-ZAeoeL5eBbg-CpUfSELLmc",
            AudioFileName = "audio",
            InputAudioFormat = "ogg",
            OutputAudioFormat = "wav", // Новое поле
            InputAudioBitrate = 48000,
        };
    }
}