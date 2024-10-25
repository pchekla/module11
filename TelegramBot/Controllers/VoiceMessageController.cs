using Telegram.Bot;
using Telegram.Bot.Types;
using Bot.Configuration;
using Bot.Services;

namespace Bot.Controllers;

public class VoiceMessageController(AppSettings appSettings, ITelegramBotClient telegramBotClient, IFileHandler audioFileHandler)
{
    private readonly AppSettings _appSettings = appSettings;
    private readonly ITelegramBotClient _telegramClient = telegramBotClient;
    private readonly IFileHandler _audioFileHandler = audioFileHandler;

    public async Task Handle(Message message, CancellationToken ct)
    {
        var fileId = message.Voice?.FileId;
        if (fileId == null)
            return;

        await _audioFileHandler.Download(fileId, ct);

        await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Голосовое сообзщение загружено", cancellationToken: ct);
    }
}