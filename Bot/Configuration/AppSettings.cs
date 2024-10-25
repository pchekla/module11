namespace Bot.Configuration;

public class AppSettings
{
    public required string DownloadsFolder { get; set; }
    public required string BotToken { get; set; }
    public required string AudioFileName { get; set; }
    public required string InputAudioFormat { get; set; }
    public required string OutputAudioFormat { get; set; }
    public float InputAudioBitrate { get; set; }
}