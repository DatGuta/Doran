using DR.Helper;

namespace DR.Models.Dto {

    public class CustomerGroupDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NumberOfCustomers { get; set; }

        public Database.Models.CustomerGroup ToEntity(string merchantId) {
            return new Database.Models.CustomerGroup() {
                Id = NGuidHelper.New(Id),
                MerchantId = merchantId,
                Code = Code,
                Name = Name,
                Description = Description,
                SearchName = StringHelper.UnsignedUnicode(Name),
                CreatedDate = DateTimeOffset.UtcNow,
                ModifiedDate = DateTimeOffset.UtcNow,
            };
        }

        [return: NotNullIfNotNull(nameof(entity))]
        public static CustomerGroupDto? FromEntity(Database.Models.CustomerGroup? entity) {
            if (entity == null) return null;
            return new CustomerGroupDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                NumberOfCustomers = entity.NumberOfCustomers,
            };
        }
    }
}
