using Telegram.Bot;
using Telegram.Bot.Types;
using Bot.Services;

namespace Bot.Controllers;

public class VoiceMessageController(IStorage memoryStorage, ITelegramBotClient telegramBotClient, IFileHandler audioFileHandler)
{
    private readonly IStorage _memoryStorage = memoryStorage;

    private readonly ITelegramBotClient _telegramClient = telegramBotClient;
    private readonly IFileHandler _audioFileHandler = audioFileHandler;

    public async Task Handle(Message message, CancellationToken ct)
    {
        var fileId = message.Voice?.FileId;
        if (fileId == null)
            return;

        await _audioFileHandler.Download(fileId, ct);

        string userLanguageCode = _memoryStorage.GetSession(message.Chat.Id).LanguageCode; // Здесь получим язык из сессии пользователя
        var result = _audioFileHandler.Process(userLanguageCode); // Запустим обработку
        await _telegramClient.SendTextMessageAsync(message.Chat.Id, result, cancellationToken: ct);
    }
}