using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Bot.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Controllers;

public class InlineKeyboardController
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly IStorage _memoryStorage;

    public InlineKeyboardController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
    {
        _telegramClient = telegramBotClient;
        _memoryStorage = memoryStorage;
    }

    public async Task Handle(CallbackQuery? callbackQuery, CancellationToken ct)
    {
        if (callbackQuery?.Data == null)
            return;

        // Получаем сессию пользователя
        var session = _memoryStorage.GetSession(callbackQuery.From.Id);

        switch (callbackQuery.Data)
        {
            case "char_count":
                session.SelectedAction = "Подсчитать символы в тексте";
                await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id, "Отправьте текст для подсчета символов.", cancellationToken: ct);
                break;

            case "sum_calc":
                session.SelectedAction = "Вычислить сумму чисел";
                await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id, "Отправьте текст для вычисления суммы.", cancellationToken: ct);
                break;

            case "audio_transcribe":
                session.SelectedAction = "audio_transcribe";
                var languageButtons = new List<InlineKeyboardButton[]>
                {
                    new[] { InlineKeyboardButton.WithCallbackData(" Русский", "ru"), InlineKeyboardButton.WithCallbackData(" English", "en"), InlineKeyboardButton.WithCallbackData(" Français", "fr") }
                };
                await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id, "Выберите язык для расшифровки аудио:", cancellationToken: ct, replyMarkup: new InlineKeyboardMarkup(languageButtons));
                break;

            case "ru":
            case "en":
            case "fr":
                session.LanguageCode = callbackQuery.Data;
                await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id,
                    $"<b>Язык аудио - {(callbackQuery.Data == "ru" ? "Русский" : callbackQuery.Data == "en" ? "Английский" : "Французский")}.</b>{Environment.NewLine}Теперь отправьте аудиосообщение для расшифровки.",
                    cancellationToken: ct, parseMode: ParseMode.Html);
                break;

            default:
                await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id, "Неверный выбор. Попробуйте снова.", cancellationToken: ct);
                break;
        }
    }
}