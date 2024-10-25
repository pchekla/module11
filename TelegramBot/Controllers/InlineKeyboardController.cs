using Telegram.Bot;
using Telegram.Bot.Types;


namespace Bot.Controllers;

public class InlineKeyboardController(ITelegramBotClient telegramBotClient)
{
    private readonly ITelegramBotClient _telegramClient = telegramBotClient;

    public async Task Handle(CallbackQuery? callbackQuery, CancellationToken ct)
    {
        Console.WriteLine($"Контроллер {GetType().Name} обнаружил нажатие на кнопку");

        await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id, $"Обнаружено нажатие на кнопку", cancellationToken: ct);
    }
}