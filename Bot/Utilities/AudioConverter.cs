using FFMpegCore;
using System.IO;

namespace Bot.Utilities 
{
    public static class AudioConverter 
    {
        public static void TryConvert(string inputFile, string outputFile) 
        {
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new ArgumentNullException(nameof(inputFile), "Входной файл не может быть null или пустым."); // Проверка на null или пустую строку

            if (string.IsNullOrWhiteSpace(outputFile))
                throw new ArgumentNullException(nameof(outputFile), "Выходной файл не может быть null или пустым."); // Проверка на null или пустую строку

            // Задаём путь, где лежит вспомогательная программа - конвертер
            GlobalFFOptions.Configure(options => options.BinaryFolder = "/usr/bin");

            // Вызываем Ffmpeg, передав требуемые аргументы.
            FFMpegArguments
                .FromFileInput(inputFile)
                .OutputToFile(outputFile, true, options => options.WithFastStart())
                .ProcessSynchronously();
        }

        private static string GetSolutionRoot()
        {
            var dir = Directory.GetCurrentDirectory();
            if (string.IsNullOrWhiteSpace(dir))
                throw new InvalidOperationException("Не удалось получить текущую директорию."); // Проверка на null или пустую строку

            var parentDir = Directory.GetParent(dir);
            if (parentDir == null)
                throw new InvalidOperationException("Не удалось получить родительский каталог."); // Проверка на null

            var fullname = parentDir.FullName;
            var projectRoot = fullname.Substring(0, fullname.Length - 4);
            
            var solutionRoot = Directory.GetParent(projectRoot);
            if (solutionRoot == null)
                throw new InvalidOperationException("Не удалось получить корень решения."); // Проверка на null

            return solutionRoot.FullName;
        }
    }
}
