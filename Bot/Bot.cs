using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Bot.Controllers;

namespace Bot;
internal class Bot(
    ITelegramBotClient telegramClient,
    InlineKeyboardController inlineKeyboardController,
    TextMessageController textMessageController,
    VoiceMessageController voiceMessageController,
    DefaultMessageController defaultMessageController) : BackgroundService
{
    // Клиент к Telegram Bot API
    private ITelegramBotClient _telegramClient = telegramClient;

    // Контроллеры различных видов сообщений
    private InlineKeyboardController _inlineKeyboardController = inlineKeyboardController;
    private TextMessageController _textMessageController = textMessageController;
    private VoiceMessageController _voiceMessageController = voiceMessageController;
    private DefaultMessageController _defaultMessageController = defaultMessageController;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _telegramClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            new ReceiverOptions() { AllowedUpdates = { } }, // Здесь выбираем, какие обновления хотим получать. В данном случае - разрешены все
            cancellationToken: stoppingToken);

        Console.WriteLine("Бот запущен.");
        return Task.CompletedTask;
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        //  Обрабатываем нажатия на кнопки  из Telegram Bot API: https://core.telegram.org/bots/api#callbackquery
        if (update.Type == UpdateType.CallbackQuery)
        {
            await _inlineKeyboardController.Handle(update.CallbackQuery, cancellationToken);
            return;
        }

        // Обрабатываем входящие сообщения из Telegram Bot API: https://core.telegram.org/bots/api#message
        if (update.Type == UpdateType.Message)
        {
            switch (update.Message!.Type)
            {
                case MessageType.Voice:
                    await _voiceMessageController.Handle(update.Message, cancellationToken);
                    return;
                case MessageType.Text:
                    await _textMessageController.Handle(update.Message, cancellationToken);
                    return;
                default:
                    await _defaultMessageController.Handle(update.Message, cancellationToken);
                    return;
            }
        }
    }

    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        Console.WriteLine("Ожидаем 10 секунд перед повторным подключением.");
        Thread.Sleep(10000);

        return Task.CompletedTask;
    }
}