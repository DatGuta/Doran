using DR.Common.Lock;
using DR.Constant.Enums;
using DR.Contexts.NormalContexts;
using DR.Database.Models;

namespace FMS.ManageApi.Application.Consumers.NormalConsumers;

internal class UpdateProductTrackingConsumer(IServiceProvider serviceProvider)
    : BaseRabbitMqConsumer<UpdateProductTrackingContext>(serviceProvider) {

    public override async Task Handle(UpdateProductTrackingContext context, CancellationToken cancellationToken) {
        if (string.IsNullOrWhiteSpace(context.MerchantId)
                || string.IsNullOrWhiteSpace(context.WarehouseId)
                || string.IsNullOrWhiteSpace(context.ProductId)) {
            return;
        }

        string key = $"{nameof(UpdateProductTrackingConsumer)}:{context.MerchantId}:{context.WarehouseId}:{context.ProductId}";

        await Locker.LockByKey(key, () => Process(context, cancellationToken), expirySec: 300);
    }

    private async Task Process(UpdateProductTrackingContext context, CancellationToken cancellationToken) {
        using var transaction = await this.db.Database.BeginTransactionAsync(cancellationToken);
        try {
            var nonUpdateItem = await this.db.ProductTrackings.AsNoTracking()
                .Where(o => o.MerchantId == context.MerchantId
                    && o.WarehouseId == context.WarehouseId
                    && o.ProductId == context.ProductId
                    && !o.IsUpdateOnHand)
                .OrderBy(o => o.Date)
                .FirstOrDefaultAsync(cancellationToken);
            if (nonUpdateItem == null) return;

            var warehouse = await this.db.Warehouses.AsNoTracking()
                .Where(o => o.MerchantId == context.MerchantId
                    && o.Id == context.WarehouseId)
                .FirstOrDefaultAsync(cancellationToken);
            if (warehouse == null) return;

            var productTrackings = await this.db.ProductTrackings
                .Where(o => o.MerchantId == context.MerchantId
                    && o.WarehouseId == context.WarehouseId
                    && o.ProductId == context.ProductId
                    && (nonUpdateItem.Date <= o.Date || o.Id == nonUpdateItem.Id))
                .OrderBy(o => o.Date)
                    .ThenBy(o => o.DocumentType)
                    .ThenBy(o => o.DocumentCode.Length)
                    .ThenBy(o => o.DocumentCode)
                    .ThenBy(o => o.ItemId)
                .ToListAsync(cancellationToken);

            if (warehouse.Type == EWarehouse.Normal) {
                var productOnWarehouse = await this.db.ProductOnWarehouses
                    .Where(o => o.MerchantId == context.MerchantId
                        && o.WarehouseId == context.WarehouseId
                        && o.ProductId == context.ProductId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (productOnWarehouse == null) return;

                var lastPt = await this.db.ProductTrackings.AsNoTracking()
                    .Where(o => o.MerchantId == context.MerchantId
                        && o.WarehouseId == context.WarehouseId
                        && o.ProductId == context.ProductId
                        && o.IsUpdateOnHand
                        && o.Date < nonUpdateItem.Date)
                    .OrderByDescending(o => o.Date)
                    .Select(o => new { o.OnHandAfter, o.Date })
                    .FirstOrDefaultAsync(cancellationToken);

                HandleProductTrackingWithNormalWarehouse(productTrackings,
                    productOnWarehouse,
                    lastPt?.OnHandAfter ?? decimal.Zero,
                    lastPt?.Date ?? DateTimeOffset.UnixEpoch);
            } else if (warehouse.Type == EWarehouse.Ticket) {
                HandleProductTrackingWithTicketWarehouse(productTrackings);
            }

            await this.db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        } catch {
            await transaction.RollbackAsync(cancellationToken);
        }
    }

    private static void HandleProductTrackingWithNormalWarehouse(
        List<ProductTracking> productTrackings,
        ProductOnWarehouse productOnWarehouse,
        decimal onHandAfter,
        DateTimeOffset dateAfter) {
        foreach (var productTracking in productTrackings) {
            productTracking.OnHandBefore = onHandAfter;
            onHandAfter = CalculateOnHand(onHandAfter, productTracking);
            productTracking.OnHandAfter = onHandAfter;
            productTracking.IsUpdateOnHand = true;

            if (productTracking.Date <= dateAfter) {
                productTracking.Date = dateAfter.AddMilliseconds(1);
            }
            dateAfter = productTracking.Date;
        }

        productOnWarehouse.OnHand = onHandAfter;
        productOnWarehouse.ModifiedDate = DateTimeOffset.UtcNow;
    }

    private static void HandleProductTrackingWithTicketWarehouse(List<ProductTracking> productTrackings) {
        foreach (var productTracking in productTrackings) {
            productTracking.OnHandBefore = productTracking.Quantity;
            productTracking.OnHandAfter = productTracking.Quantity;
            productTracking.IsUpdateOnHand = true;
        }
    }

    private static decimal CalculateOnHand(decimal onHand, ProductTracking productTracking) {
        if (productTracking.IsDelete) {
            return onHand;
        }
        return onHand + productTracking.Quantity;
    }
}
