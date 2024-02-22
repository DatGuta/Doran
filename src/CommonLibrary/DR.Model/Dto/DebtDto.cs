namespace DR.Models.Dto {
    public class DebtDto {
        public string Id { get; set; } = string.Empty;

        public EDebtItem ItemType { get; set; }
        public decimal Value { get; set; }
        public string? Description { get; set; }

        public bool IsOriginal { get; set; }
        public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;

        public List<DebtItemDto> Items { get; set; } = new();

        public static DebtDto FromEntity(Database.Models.Debt entity) {
            return new DebtDto {
                Id = entity.Id,
                ItemType = entity.ItemType,
                Value = entity.Value,
                Description = entity.Description,
                IsOriginal = entity.IsOriginal,
                Time = entity.Time,
            };
        }
    }
}
