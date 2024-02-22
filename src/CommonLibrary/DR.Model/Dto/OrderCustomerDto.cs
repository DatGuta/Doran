namespace DR.Models.Dto {
    public class OrderCustomerDto {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }

        public string GetFullAddress() {
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(Address))
                list.Add(Address);
            if (!string.IsNullOrWhiteSpace(Commune))
                list.Add(Commune);
            if (!string.IsNullOrWhiteSpace(District))
                list.Add(District);
            if (!string.IsNullOrWhiteSpace(Province))
                list.Add(Province);
            return string.Join(", ", list);
        }

        [return: NotNullIfNotNull(nameof(entity))]
        public static OrderCustomerDto? FromEntity(Database.Models.OrderCustomer? entity, UnitResource? unitRes) {
            if (entity == null) return default;
            return new OrderCustomerDto {
                Id = entity.Id,
                Name = entity.Name,
                Phone = PhoneHelper.Decrypt(entity.Phone) ?? string.Empty,
                Province = unitRes?.GetByCode(entity.Province)?.Name,
                District = unitRes?.GetByCode(entity.District)?.Name,
                Commune = unitRes?.GetByCode(entity.Commune)?.Name,
                Address = entity.Address,
            };
        }
    }
}
