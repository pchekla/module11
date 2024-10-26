using System.Text;
using Newtonsoft.Json.Linq;
using Bot.Extensions;
using Vosk;

namespace Bot.Utilities
{
    public static class SpeechDetector
    {
        public static string DetectSpeech(string audioPath, float inputBitrate, string languageCode)
        {
            if (string.IsNullOrWhiteSpace(audioPath))
                throw new ArgumentNullException(nameof(audioPath), "Путь к аудио не может быть null или пустым."); // Проверка на null или пустую строку

            if (string.IsNullOrWhiteSpace(languageCode))
                throw new ArgumentNullException(nameof(languageCode), "Код языка не может быть null или пустым."); // Проверка на null или пустую строку

            Vosk.Vosk.SetLogLevel(-1);

            var projectRoot = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
            var modelPath = Path.Combine(projectRoot ?? throw new InvalidOperationException("Project root not found"), "Speech-models", $"vosk-model-small-{languageCode.ToLower()}");

            Model model = new(modelPath);
            return GetWords(model, audioPath, inputBitrate);
        }

        /// <summary>
        /// Основной метод для распознавания слов
        /// </summary>
        /// 
        private static string GetWords(Model model, string audioPath, float inputBitrate)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Модель не может быть null."); // Проверка на null для модели

            if (string.IsNullOrWhiteSpace(audioPath))
                throw new ArgumentNullException(nameof(audioPath), "Путь к аудио не может быть null или пустым."); // Проверка на null или пустую строку

            // В конструктор для распознавания передаем битрейт, а также используемую языковую модель
            VoskRecognizer rec = new(model, inputBitrate);
            rec.SetMaxAlternatives(0);
            rec.SetWords(true);

            StringBuilder textBuffer = new();

            using (Stream source = File.OpenRead(audioPath))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // Распознавание отдельных слов
                    if (rec.AcceptWaveform(buffer, bytesRead))
                    {
                        var sentenceJson = rec.Result();
                        // Сохраняем текстовый вывод в JSON-объект и извлекаем данные
                        JObject sentenceObj = JObject.Parse(sentenceJson);
                        string sentence = (string)sentenceObj["text"] ?? string.Empty; // Убедитесь, что sentence не равен null
                        textBuffer.Append(StringExtension.UppercaseFirst(sentence) + ". ");
                    }
                }
            }

            // Распознавание предложений
            var finalSentence = rec.FinalResult();
            JObject finalSentenceObj = JObject.Parse(finalSentence);
            textBuffer.Append((string)finalSentenceObj["text"] ?? string.Empty); // Обрабатываем возможное значение null

            if (textBuffer.Length == 0)
            {
                return "Ошибка при распознавании речи.";
            }

            return textBuffer.ToString();
        }
    }
}