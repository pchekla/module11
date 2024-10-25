namespace Bot.Configuration;

public class AppSettings
{

    public string DownloadsFolder { get; set; }
    public string BotToken { get; set; }
    public string AudioFileName { get; set; }
    public string InputAudioFormat { get; set; }
    public string OutputAudioFormat { get; set; }
    public long InputAudioBitrate { get; set; }
}