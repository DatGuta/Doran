using Newtonsoft.Json.Linq;

namespace DR.Models.Dto {
    public class SettingDto {
        public ESetting Code { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public JToken Value { get; set; } = default!;

        [return: NotNullIfNotNull(nameof(entity))]
        public static SettingDto? FromEntity(Database.Models.GeneralSetting? entity, Database.Models.MerchantSetting? merchantSetting) {
            if (entity == null) return default;

            return new SettingDto {
                Code = entity.Code,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
                Value = merchantSetting?.Value ?? entity.DefaultValue,
            };
        }
    }
}
