namespace DR.Models.Dto {

    public class WarehouseTransferDto {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public WarehouseDto? FromWarehouse { get; set; }
        public WarehouseDto? ToWarehouse { get; set; }
        public EWarehouseTransfer Status { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? ExportedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ImportedDate { get; set; } = DateTimeOffset.UtcNow;
        public UserDto? CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public UserDto? UpdatedBy { get; set; }
        public DateTimeOffset LastUpdateDate { get; set; } = DateTimeOffset.UtcNow;
        public List<WarehouseTransferDetailDto> Items { get; set; } = new();

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseTransferDto? FromEntity(WarehouseTransfer? entity,
            UnitResource unitRes) {
            if (entity == null) return default;

            return new WarehouseTransferDto {
                Id = entity.Id,
                Code = entity.Code,
                FromWarehouse = WarehouseDto.FromEntity(entity.FromWarehouse, unitRes),
                ToWarehouse = WarehouseDto.FromEntity(entity.ToWarehouse, unitRes),
                Status = entity.Status,
                Description = entity.Description,
                ExportedDate = entity.ExportedDate,
                ImportedDate = entity.ImportedDate,
                CreatedBy = UserDto.FromEntity(entity.CreatedUser, unitRes),
                CreatedDate = entity.CreatedDate,
                UpdatedBy = UserDto.FromEntity(entity.UpdatedUser, unitRes),
                LastUpdateDate = entity.LastUpdateDate,
                Items = entity.WarehouseTransferDetails?.Select(i => WarehouseTransferDetailDto.FromEntity(i)).ToList() ?? new(),
            };
        }
    }
}
