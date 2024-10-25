using FFMpegCore;

namespace Bot.Utilities 
{
  public static class AudioConverter 
  {
    public static void TryConvert(string inputFile, string outputFile) 
    {
      // Задаём путь, где лежит вспомогательная программа - конвертер, для линукса работает напрямую с любого пути
      GlobalFFOptions.Configure(options => options.BinaryFolder = "/usr/bin");

      // Вызываем Ffmpeg, передав требуемые аргументы.
      FFMpegArguments
        .FromFileInput(inputFile)
        .OutputToFile(outputFile, true, options => options
          .WithFastStart())
        .ProcessSynchronously();
}
    private static string GetSolutionRoot()
    {
        var dir = Path.GetDirectoryName(Directory.GetCurrentDirectory());
        var fullname = Directory.GetParent(dir).FullName;
        var projectRoot = fullname.Substring(0, fullname.Length - 4);
        return Directory.GetParent(projectRoot)?.FullName;
    }
  }
}
