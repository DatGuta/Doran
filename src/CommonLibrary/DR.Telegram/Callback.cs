using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace DR.Telegram;

public class Callback {

    [JsonProperty("t"), JsonRequired]
    public int Type { get; set; }

    [JsonProperty("d", NullValueHandling = NullValueHandling.Ignore)]
    public string? Data { get; set; }

    public Callback(int type) {
        this.Type = type;
    }

    public Callback(int type, string data) {
        this.Type = type;
        this.Data = data;
    }

    public override string ToString() {
        string result = $"t:{Type}";
        if (!string.IsNullOrWhiteSpace(Data)) {
            result = $"{result}:d:{Data}";
        }
        if (result.Length > 64)
            throw new NotImplementedException("The length of callback data cannot be greater than 64 characters");
        return result;
    }

    public static Callback? Parse(string? text) {
        if (!string.IsNullOrWhiteSpace(text)) {
            var match = Regex.Match(text, "^t:(?<type>[0-9]+):d:(?<data>.*)$");
            if (match.Success) {
                int type = int.Parse(match.Groups["type"].Value);
                string? data = match.Groups["data"]?.Value;

                if (string.IsNullOrWhiteSpace(data)) {
                    return new Callback(type);
                }
                return new Callback(type, data);
            }
        }
        return null;
    }
}
