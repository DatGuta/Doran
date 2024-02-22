namespace DR.Models.Dto {
    public class DeliveryDto {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public Unit? Province { get; set; }
        public Unit? District { get; set; }
        public Unit? Commune { get; set; }
        public string? Address { get; set; }

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

        [return: NotNullIfNotNull(nameof(dto))]
        public static DeliveryDto? FromCustomerDto(CustomerDto? dto) {
            if (dto == null) return default;
            return new DeliveryDto {
                Id = dto.Id ?? string.Empty,
                Name = dto.Name,
                Phone = dto.Phone,
                Province = dto.Province,
                District = dto.District,
                Commune = dto.Commune,
                Address = dto.Address,
            };
        }

        [return: NotNullIfNotNull(nameof(entity))]
        public static DeliveryDto? FromEntity(Database.Models.OrderCustomer? entity, UnitResource? unitRes) {
            if (entity == null) return default;
            return new DeliveryDto {
                Id = entity.Id,
                Name = entity.Name,
                Phone = PhoneHelper.Decrypt(entity.Phone),
                Province = unitRes?.GetByCode(entity.Province),
                District = unitRes?.GetByCode(entity.District),
                Commune = unitRes?.GetByCode(entity.Commune),
                Address = entity.Address,
            };
        }
    }
}
