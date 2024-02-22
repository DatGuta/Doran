using Newtonsoft.Json;

namespace DR.Models.Dto {
    public class ImageDto {

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public byte[]? Data { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static ImageDto? FromEntity(ItemImage? entity, string? currentUrl) {
            if (entity == null) return default;

            return new ImageDto {
                Id = entity.Id,
                Name = entity.Name,
                Image = $"{currentUrl}/{entity.Image}",
            };
        }
    }
}
