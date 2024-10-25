using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Controllers;

public class DefaultMessageController(ITelegramBotClient telegramBotClient)
{
    private readonly ITelegramBotClient _telegramClient = telegramBotClient;

    public async Task Handle(Message message, CancellationToken ct)
    {
        Console.WriteLine($"Контроллер {GetType().Name} получил сообщение");
        await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Получено сообщение не поддерживаемого формата", cancellationToken: ct);
    }
}