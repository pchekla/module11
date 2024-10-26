using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Bot.Services;
using System.Text.RegularExpressions;

namespace Bot.Controllers;

public class TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
{
    private readonly ITelegramBotClient _telegramClient = telegramBotClient;
    private readonly IStorage _memoryStorage = memoryStorage;

    public async Task Handle(Message message, CancellationToken ct)
    {
        // Получаем сессию пользователя и последнее выбранное действие
        var session = _memoryStorage.GetSession(message.From.Id);
        var selectedAction = session.SelectedAction;

        // Обработка команды "/start"
        if (message.Text == "/start")
        {
            var buttons = new List<InlineKeyboardButton[]>
            {
                new[] { InlineKeyboardButton.WithCallbackData(" Подсчитать символы в тексте", "char_count") },
                new[] { InlineKeyboardButton.WithCallbackData(" Вычислить сумму чисел", "sum_calc") },
                new[] { InlineKeyboardButton.WithCallbackData(" Расшифровать аудио сообщение", "audio_transcribe") }
            };

            await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b>Наш бот выполняет следующие функции:</b> {Environment.NewLine}" +
                $"{Environment.NewLine}1. Подсчитывает количество символов в тексте.{Environment.NewLine}" +
                $"{Environment.NewLine}2. Вычисляет сумму чисел.{Environment.NewLine}" +
                $"{Environment.NewLine}3. Расшифровывает аудио в текст.{Environment.NewLine}" + 
                $"{Environment.NewLine}<b>Выберите необходимую опцию в меню.</b>", 
                cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
        }
        else
        {
            // Проверяем, выбрана ли функция "Расшифровать аудио сообщение"
            if (selectedAction == "audio_transcribe")
            {
                // Если сообщение не является аудио, отправляем предупреждение
                if (message.Type != MessageType.Voice && message.Type != MessageType.Audio)
                {
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Сообщение не содержит аудио. Пожалуйста, отправьте аудиосообщение для расшифровки.", cancellationToken: ct);
                    return;
                }

                // Логика для обработки аудиосообщения, если оно действительно отправлено
                await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Аудиосообщение получено! Начинаем расшифровку...", cancellationToken: ct);
                
                // Здесь можно вызвать метод для обработки и расшифровки аудио, если он имеется
                // Например, `TranscribeAudio(message.Voice.FileId);`
            }
            else
            {
                // Выполнение действий в зависимости от выбранного ранее действия
                switch (selectedAction)
                {
                    case "Подсчитать символы в тексте":
                        int charCount = message.Text.Length;
                        await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"В вашем тексте {charCount} символов.", cancellationToken: ct);
                        break;

                    case "Вычислить сумму чисел":
                        int sum = CalculateSum(message.Text);
                        string response = sum >= 0 
                            ? $"Сумма чисел в вашем сообщении: {sum}." 
                            : "Сообщение не содержит чисел для подсчета.";
                        await _telegramClient.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: ct);
                        break;

                    default:
                        await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Пожалуйста, выберите действие из меню.", cancellationToken: ct);
                        break;
                }
            }
        }
    }

    // Метод для подсчета суммы чисел в тексте
    private int CalculateSum(string text)
    {
        var matches = Regex.Matches(text, @"\d+");
        int sum = 0;
        
        foreach (Match match in matches)
        {
            sum += int.Parse(match.Value);
        }

        return sum > 0 ? sum : -1; // Возвращаем -1, если чисел не найдено
    }
}
