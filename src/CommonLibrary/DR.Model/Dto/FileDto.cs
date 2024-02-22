using Newtonsoft.Json;

namespace DR.Models.Dto {
    public class FileDto {

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? ItemId { get; set; }
        public EFile Type { get; set; }
        public string? Path { get; set; }
        public EItemType ItemType { get; set; }
        public string? UploadBy { get; set; }
        public DateTimeOffset UploadDate { get; set; } = DateTimeOffset.Now;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public byte[]? Data { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static FileDto? FromEntity(Database.Models.File? entity, string? currentUrl) {
            if (entity == null) return default;

            return new FileDto {
                Id = entity.Id,
                Name = entity.Name,
                ItemId = entity.ItemId,
                Type = entity.Type,
                ItemType = entity.ItemType,
                Path = $"{currentUrl}/{entity.Path}",
                UploadBy = entity.UploadBy,
                UploadDate = entity.UploadDate,
            };
        }
    }
}
