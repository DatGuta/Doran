namespace DR.Models.Dto {
    public class DeviceDto {
        public string Id { get; set; } = null!;
        public string SerialNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }

        public StoreDto? Store { get; set; }
        public WarehouseDto? Warehouse { get; set; }

        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;

        public static DeviceDto FromEntity(Device entity, StoreDto? store, WarehouseDto? warehouse) {
            return new DeviceDto {
                Id = entity.Id,
                SerialNumber = entity.SerialNumber,
                Name = entity.Name,
                IsActive = entity.IsActive,
                Store = store,
                Warehouse = warehouse,
                CreatedDate = entity.CreatedDate,
                ModifiedDate = entity.ModifiedDate,
            };
        }
    }
}
