namespace Bot.Models;

public class Session
{
    public string? LanguageCode { get; set; }
    public string SelectedAction { get; set; } = string.Empty;
}