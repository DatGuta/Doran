using Newtonsoft.Json;

namespace DR.Models.Dto {
    public class SupplierDto {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public ImageDto? Image { get; set; }

        public Database.Models.Supplier ToEntity(string merchantId) {
            return new Database.Models.Supplier() {
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
        public static SupplierDto? FromEntity(Database.Models.Supplier? entity,
            Database.Models.ItemImage? itemImageEntity = null,
            string? currentUrl = null) {
            if (entity == null) return default;

            return new SupplierDto {
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
