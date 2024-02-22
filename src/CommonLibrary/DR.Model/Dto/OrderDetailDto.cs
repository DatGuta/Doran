using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DR.Models.Dto {
    public class OrderDetailDto {
        public string? Id { get; set; }
        public string? GroupId { get; set; }
        public bool IsOverThreshold { get; set; }
        public int OrderIndex { get; set; }
        public ProductDto Product { get; set; } = new();
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ItemDiscount { get; set; }
        public decimal TotalItem { get; set; }

        public decimal ExportQuantity { get; set; }
        public decimal RefundQuantity { get; set; }
        public decimal RefundAmount { get; set; }

        public bool IsPromotion { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DiscountDto? Discount { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static OrderDetailDto? FromEntity(Database.Models.OrderDetail? entity) {
            if (entity == null) return default;

            return new OrderDetailDto {
                Id = entity.Id,
                GroupId = entity.GroupId,
                IsOverThreshold = entity.IsOverThreshold,
                OrderIndex = entity.OrderIndex,
                Product = ProductDto.FromEntity(entity.Product) ?? new ProductDto {
                    Id = entity.ProductId ?? string.Empty,
                    Code = entity.ProductCode ?? string.Empty,
                    Name = entity.ProductName ?? string.Empty,
                },
                Discount = new DiscountDto {
                    Id = entity.DiscountId,
                    Code = "",
                    Name = "",
                    Type = entity.DiscountType,
                    Value = entity.DiscountValue,
                },
                Price = entity.Price,
                Quantity = entity.Quantity,
                SubTotal = entity.SubTotal,
                ItemDiscount = entity.ItemDiscount,
                TotalItem = entity.TotalItem,
                ExportQuantity = entity.ExportQuantity,
                RefundQuantity = entity.RefundQuantity,
                RefundAmount = entity.RefundAmount,
                IsPromotion = entity.IsPromotion,
            };
        }
    }
}
