using Newtonsoft.Json;

namespace DR.Common.Models;

public class PaginatedList<T> {

    [JsonProperty("items")]
    public List<T> Items { get; set; } = new();

    [JsonProperty("count")]
    public int Count { get; set; }
}
