
using Newtonsoft.Json;

namespace DR.Models.Dto {

    public class BrandDto {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public ImageDto? Image { get; set; }

        public Database.Models.Brand ToEntity(string merchantId) {
            return new Database.Models.Brand() {
                Id = NGuidHelper.New(Id),
                MerchantId = merchantId,
                Code = Code ?? string.Empty,
                Name = Name,
                SearchName = StringHelper.UnsignedUnicode(Name),
                Phone = Phone,
                Email = Email,
            };
        }

        [return: NotNullIfNotNull(nameof(entity))]
        public static BrandDto? FromEntity(Database.Models.Brand? entity,
            Database.Models.ItemImage? itemImageEntity = null,
            string? currentUrl = null) {
            if (entity == null) return default;

            return new BrandDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Phone = entity.Phone,
                Email = entity.Email,
                Image = ImageDto.FromEntity(itemImageEntity, currentUrl),
            };
        }
    }
}
