namespace DR.Models.Dto {
    public class CustomerDto {
        public string? Id { get; set; }
        public CustomerGroupDto? CustomerGroup { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public Unit? Province { get; set; }
        public Unit? District { get; set; }
        public Unit? Commune { get; set; }
        public string? Address { get; set; }
        public DateTimeOffset? LastPurchase { get; set; }

        public string GetFullAddress() {
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(Address))
                list.Add(Address);
            if (!string.IsNullOrWhiteSpace(Commune?.Name))
                list.Add(Commune.Name);
            if (!string.IsNullOrWhiteSpace(District?.Name))
                list.Add(District.Name);
            if (!string.IsNullOrWhiteSpace(Province?.Name))
                list.Add(Province.Name);
            return string.Join(", ", list);
        }

        public Database.Models.Customer ToEntity(string merchantId) {
            return new Database.Models.Customer() {
                Id = NGuidHelper.New(Id),
                CustomerGroupId = CustomerGroup?.Id,
                MerchantId = merchantId,
                Code = Code!,
                Name = Name,
                SearchName = StringHelper.UnsignedUnicode(Name),
                Phone = PhoneHelper.Encrypt(Phone),
                Province = Province?.Code,
                District = District?.Code,
                Commune = Commune?.Code,
                Address = Address,
            };
        }

        [return: NotNullIfNotNull(nameof(entity))]
        public static CustomerDto? FromEntity(Database.Models.Customer? entity, UnitResource? unitRes) {
            if (entity == null) return default;
            return new CustomerDto {
                Id = entity.Id,
                CustomerGroup = CustomerGroupDto.FromEntity(entity.CustomerGroup),
                Code = entity.Code,
                Name = entity.Name,
                Phone = PhoneHelper.Decrypt(entity.Phone),
                Province = unitRes?.GetByCode(entity.Province),
                District = unitRes?.GetByCode(entity.District),
                Commune = unitRes?.GetByCode(entity.Commune),
                Address = entity.Address,
                LastPurchase = entity.LastPurchase >= 0 ? DateTimeOffset.FromUnixTimeMilliseconds(entity.LastPurchase) : null,
            };
        }
    }
}
