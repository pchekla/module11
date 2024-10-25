using System.IO;

namespace Bot.Extensions;

public static class DirectoryExtension
{
    /// <summary>
    /// Получаем путь до каталога с .sln файлом
    /// </summary>
    public static string GetSolutionRoot()
    {
        // Получаем текущую директорию
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
