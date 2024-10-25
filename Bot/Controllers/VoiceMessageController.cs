using Telegram.Bot;
using Telegram.Bot.Types;
using Bot.Services;

namespace Bot.Controllers;

public class VoiceMessageController
{
    private readonly IStorage _memoryStorage;
    private readonly ITelegramBotClient _telegramClient;
    private readonly IFileHandler _audioFileHandler;

    public VoiceMessageController(IStorage memoryStorage, ITelegramBotClient telegramBotClient, IFileHandler audioFileHandler)
    {
        _memoryStorage = memoryStorage ?? throw new ArgumentNullException(nameof(memoryStorage));
        _telegramClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
        _audioFileHandler = audioFileHandler ?? throw new ArgumentNullException(nameof(audioFileHandler));
    }

    public async Task Handle(Message message, CancellationToken ct)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message)); // Проверка на null для message

        var fileId = message.Voice?.FileId;
        if (fileId == null)
            return; // Если fileId равен null, завершаем метод

        await _audioFileHandler.Download(fileId, ct);

        var session = _memoryStorage.GetSession(message.Chat.Id);
        if (session == null)
        {
            await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Сессия не найдена.", cancellationToken: ct);
            return;
        }

        string userLanguageCode = session.LanguageCode; // Получим язык из сессии пользователя

        if (string.IsNullOrWhiteSpace(userLanguageCode))
            throw new ArgumentNullException(nameof(userLanguageCode), "Код языка не может быть null или пустым."); // Проверка на null или пустую строку

        var result = _audioFileHandler.Process(userLanguageCode); // Запустим обработку

        if (string.IsNullOrWhiteSpace(result))
        {
            await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Распознавание не удалось. Попробуйте снова.", cancellationToken: ct);
        }
        else
        {
            await _telegramClient.SendTextMessageAsync(message.Chat.Id, result, cancellationToken: ct);
        }
    }
}