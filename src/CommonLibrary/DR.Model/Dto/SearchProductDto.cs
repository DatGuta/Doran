using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DR.Models.Dto {

    public class SearchProductDto {
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal NetWeight { get; set; }
        public bool IsPromotion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string>? SupplierIds { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<SearchProductOnWarehouseDto>? OnWarehouses { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static SearchProductDto? FromEntity(
            Database.Models.Product? entity,
            bool includeOnHand) {
            if (entity == null) return default;

            return new() {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                NetWeight = entity.NetWeight,
                IsPromotion = entity.IsPromotion,
                OnWarehouses = includeOnHand ? entity.ProductOnWarehouses?.Select(o => SearchProductOnWarehouseDto.FromEntity(o)).ToList() : default,
            };
        }
    }
}
