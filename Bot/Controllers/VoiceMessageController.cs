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
            throw new ArgumentNullException(nameof(message));

        // Проверка на наличие объекта Chat
        if (message.Chat == null)
            throw new ArgumentNullException(nameof(message.Chat), "Chat не может быть null.");

        // Явная проверка на наличие объекта Voice
        if (message.Voice == null)
            throw new ArgumentNullException(nameof(message.Voice), "Voice не может быть null.");

        var voice = message.Voice; // Теперь мы уверены, что voice не равен null
        var fileId = voice.FileId ?? throw new ArgumentNullException(nameof(voice.FileId), "FileId не может быть null.");

        await _audioFileHandler.Download(fileId, ct);
        
        var session = _memoryStorage.GetSession(message.Chat.Id);
        if (session == null)
        {
            await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Сессия не найдена.", cancellationToken: ct);
            return;
        }

        string userLanguageCode = session.LanguageCode;
        if (string.IsNullOrWhiteSpace(userLanguageCode))
            throw new ArgumentNullException(nameof(userLanguageCode), "Код языка не может быть null или пустым.");

        var result = _audioFileHandler.Process(userLanguageCode);

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