namespace DR.Models.Dto {
    public class StockEntityDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public static StockEntityDto FromEntity(Product entity) {
            return new StockEntityDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
            };
        }

        public static StockEntityDto FromEntity(Warehouse entity) {
            return new StockEntityDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
            };
        }

        public static StockEntityDto? FromEntity(Supplier? entity) {
            if (entity == null) return null;
            return new StockEntityDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
            };
        }
    }
}
