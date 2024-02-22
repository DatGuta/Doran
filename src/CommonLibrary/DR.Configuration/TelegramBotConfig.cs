namespace DR.Configuration;

public class TelegramBotConfig {
    public long Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool IsListen { get; set; }
    public List<long> ChatIds { get; set; } = new();

    public string AccessToken => $"{Id}:{Token}";
}
