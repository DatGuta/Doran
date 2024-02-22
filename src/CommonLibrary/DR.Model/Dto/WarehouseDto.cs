namespace DR.Models.Dto {
    public class WarehouseDto {
        public string? Id { get; set; }
        public EWarehouse Type { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = null!;
        public Unit? Province { get; set; }
        public Unit? District { get; set; }
        public Unit? Commune { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsEditType { get; set; }

        public Database.Models.Warehouse ToEntity(string merchantId) {
            return new Database.Models.Warehouse() {
                Id = NGuidHelper.New(Id),
                MerchantId = merchantId,
                Type = Type,
                Code = Code!,
                Name = Name,
                SearchName = StringHelper.UnsignedUnicode(Name),
                Phone = Phone,
                Email = Email,
                Province = Province?.Code,
                District = District?.Code,
                Commune = Commune?.Code,
                Address = Address,
                IsActive = IsActive,
            };
        }

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseDto? FromEntity(Database.Models.Warehouse? entity,
            UnitResource? unitRes, bool isEditType = false) {
            if (entity == null) return default;
            var au = unitRes?.GetByCode(entity.Province, entity.District, entity.Commune) ?? new();
            return new WarehouseDto {
                Id = entity.Id,
                Type = entity.Type,
                Code = entity.Code,
                Name = entity.Name,
                Province = !string.IsNullOrWhiteSpace(entity.Province) && au.TryGetValue(entity.Province, out var province) ? province : default,
                District = !string.IsNullOrWhiteSpace(entity.District) && au.TryGetValue(entity.District, out var district) ? district : default,
                Commune = !string.IsNullOrWhiteSpace(entity.Commune) && au.TryGetValue(entity.Commune, out var commune) ? commune : default,
                Address = entity.Address,
                Phone = entity.Phone,
                Email = entity.Email,
                IsActive = entity.IsActive,
                IsEditType = isEditType
            };
        }
    }
}
