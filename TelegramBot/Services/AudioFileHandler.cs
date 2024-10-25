using Bot.Configuration;
using Telegram.Bot;

namespace Bot.Services;

public class AudioFileHandler(ITelegramBotClient telegramBotClient, AppSettings appSettings) : IFileHandler
{
    private readonly AppSettings _appSettings = appSettings;
    private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;

    public async Task Download(string fileId, CancellationToken ct)
    {
        // Генерируем полный путь файла из конфигурации
        string inputAudioFilePath = Path.Combine(_appSettings.DownloadsFolder, $"{_appSettings.AudioFileName}.{_appSettings.InputAudioFormat}");

        using (FileStream destinationStream = File.Create(inputAudioFilePath))
        {
            // Загружаем информацию о файле
            var file = await _telegramBotClient.GetFileAsync(fileId, ct);
            if (file.FilePath == null)
                return;

            // Скачиваем файл
            await _telegramBotClient.DownloadFileAsync(file.FilePath, destinationStream, ct);
        }
    }

    public string Process(string languageCode)
    {
        // Метод пока не реализован
        throw new NotImplementedException();
    }
}