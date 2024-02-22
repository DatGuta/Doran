namespace DR.Models.Dto {
    public class StoreDto {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = null!;
        public Unit? Province { get; set; }
        public Unit? District { get; set; }
        public Unit? Commune { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }

        public WarehouseDto? Warehouse { get; set; }

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

        public Database.Models.Store ToEntity(string merchantId) {
            return new Database.Models.Store {
                Id = NGuidHelper.New(Id),
                MerchantId = merchantId,
                WarehouseId = Warehouse?.Id,
                Code = Code!,
                Name = Name,
                SearchName = StringHelper.UnsignedUnicode(Name),
                Province = Province?.Code,
                District = District?.Code,
                Commune = Commune?.Code,
                Address = Address,
                Phone = Phone,
                Email = Email,
                IsActive = IsActive,
            };
        }

        [return: NotNullIfNotNull(nameof(entity))]
        public static StoreDto? FromEntity(Database.Models.Store? entity, UnitResource? unitRes) {
            if (entity == null) return default;

            return new StoreDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Province = unitRes?.GetByCode(entity.Province),
                District = unitRes?.GetByCode(entity.District),
                Commune = unitRes?.GetByCode(entity.Commune),
                Address = entity.Address,
                Phone = entity.Phone,
                Email = entity.Email,
                IsActive = entity.IsActive,
                Warehouse = WarehouseDto.FromEntity(entity.Warehouse, unitRes),
            };
        }
    }
}
