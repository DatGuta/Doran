using Newtonsoft.Json;

namespace DR.Common.Models;

public class Result {

    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
    public string? Message { get; set; }

    public static Result Ok() {
        return new() { Success = true };
    }

    public static Result Fail(string? message = null) {
        return new() { Message = message };
    }

    public override string ToString() {
        return JsonConvert.SerializeObject(this);
    }
}

public class Result<T> : Result {

    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public T? Data { get; set; }

    public static Result<T> Ok(T? data) {
        return new() { Success = true, Data = data };
    }
}

public class FileResult {
    public string FileName { get; set; } = string.Empty;
    public byte[] ByteArray { get; set; } = Array.Empty<byte>();
}
