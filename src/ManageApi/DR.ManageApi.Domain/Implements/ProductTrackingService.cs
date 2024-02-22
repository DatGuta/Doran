using DR.Common.Models;
using DR.Constant.Enums;
using DR.Contexts.NormalContexts;
using DR.Database;
using DR.Database.Models;
using DR.Helper;
using DR.ManageApi.Domain.Services.Interfaces;
using MassTransit;

namespace FMS.ManageApi.Domain.Services.Implements;

public class ProductTrackingService(IServiceProvider serviceProvider) : IProductTrackingService {
    private readonly DrContext db = serviceProvider.GetRequiredService<DrContext>();
    private readonly IBus bus = serviceProvider.GetRequiredService<IBusControl>();

    public async Task Save(string merchantId, string warehouseId,
            EProductDocumentType docType, string docId, string docCode,
            List<PtItem> items, CancellationToken cancellationToken) {
        if (string.IsNullOrWhiteSpace(warehouseId)
            || string.IsNullOrWhiteSpace(docId)
            || items == null
            || items.Count == 0) {
            return;
        }

        var tableNames = items.Select(o => o.TableName).Distinct().ToList();
        var pts = await this.db.ProductTrackings
            .Where(o => o.MerchantId == merchantId && o.WarehouseId == warehouseId
                && o.DocumentType == docType && o.DocumentId == docId
                && tableNames.Contains(o.TableName))
            .ToListAsync(cancellationToken);

        var contexts = new HashSet<UpdateProductTrackingContext>();
        if (pts.Count > 0) {
            var grpProdPts = pts.GroupBy(o => o.ProductId)
                .Select(o => o.MinBy(o => o.Date))
                .Where(o => o != null)
                .Select(o => o!).ToList();

            var queryPts = this.db.ProductTrackings
                .Where(o => o.MerchantId == merchantId && o.WarehouseId == warehouseId);

            var nextPtItems = grpProdPts.Join(queryPts.AsNoTracking(),
                    o => o.ProductId,
                    p => p.ProductId,
                    (o, p) => new { o, p })
                .Where(o => o.o.Date <= o.p.Date && o.o.Id != o.p.Id)
                .Select(o => o.p)
                .GroupBy(o => o.ProductId)
                .Select(o => new {
                    ProductId = o.Key,
                    Date = o.Min(x => x.Date)
                }).ToList();

            var nextPts = nextPtItems.Join(queryPts,
                    o => new { o.ProductId, o.Date },
                    p => new { p.ProductId, p.Date },
                    (o, p) => new { o, p })
                .Select(o => o.p).ToList();

            foreach (var item in nextPts) {
                item.IsUpdateOnHand = false;
                contexts.Add(new UpdateProductTrackingContext(merchantId, warehouseId, item.ProductId));
            }
        }

        var newPts = new List<ProductTracking>();
        var updatePtIds = new List<string>();
        foreach (var item in items) {
            var pt = pts.Find(o => o.ProductId == item.ProductId
                && o.TableName == item.TableName
                && o.ItemId == item.ItemId);
            if (pt != null) {
                pt.Quantity = item.Value;
                pt.Date = item.Time ?? DateTimeOffset.UtcNow;
                pt.IsUpdateOnHand = false;
                pt.IsDelete = item.IsDelete;

                updatePtIds.Add(pt.Id);

                contexts.Add(new UpdateProductTrackingContext(merchantId, warehouseId, pt.ProductId));
            } else if (!item.IsDelete) {
                newPts.Add(new ProductTracking {
                    Id = NGuidHelper.New(),
                    MerchantId = merchantId,
                    WarehouseId = warehouseId,
                    ProductId = item.ProductId,
                    DocumentType = docType,
                    DocumentId = docId,
                    DocumentCode = docCode,
                    Quantity = item.Value,
                    Date = item.Time ?? DateTimeOffset.UtcNow,
                    IsUpdateOnHand = false,
                    TableName = item.TableName,
                    ItemId = item.ItemId,
                    IsDelete = false,
                });
                contexts.Add(new UpdateProductTrackingContext(merchantId, warehouseId, item.ProductId));
            }
        }

        var removePts = pts.Where(o => !updatePtIds.Contains(o.Id)).ToList();
        foreach (var removePt in removePts) {
            if (removePt.IsDelete) continue;

            removePt.IsUpdateOnHand = false;
            removePt.IsDelete = true;

            contexts.Add(new UpdateProductTrackingContext(merchantId, warehouseId, removePt.ProductId));
        }

        await this.db.ProductTrackings.AddRangeAsync(newPts, cancellationToken);
        await this.db.SaveChangesAsync(cancellationToken);

        await Publish(contexts, cancellationToken);
    }

    public async Task Remove(string merchantId, string warehouseId,
        EProductDocumentType docType, string docId, string docCode,
        List<PtItem> items, CancellationToken cancellationToken) {
        if (string.IsNullOrWhiteSpace(warehouseId)
            || string.IsNullOrWhiteSpace(docId)
            || items == null
            || !items.Any()) {
            return;
        }

        var tableNames = items.Select(o => o.TableName).Distinct().ToList();
        var pts = await this.db.ProductTrackings
            .Where(o => o.MerchantId == merchantId && o.WarehouseId == warehouseId
                && o.DocumentType == docType && o.DocumentId == docId
                && tableNames.Contains(o.TableName) && !o.IsDelete)
            .ToListAsync(cancellationToken);

        var contexts = new HashSet<UpdateProductTrackingContext>();
        foreach (var item in items) {
            var pt = pts.Find(o => o.ProductId == item.ProductId
                && o.TableName == item.TableName
                && o.ItemId == item.ItemId);
            if (pt != null) {
                pt.IsUpdateOnHand = false;
                pt.IsDelete = true;

                contexts.Add(new UpdateProductTrackingContext(merchantId, warehouseId, pt.ProductId));
            }
        }

        await this.db.SaveChangesAsync(cancellationToken);

        if (contexts.Count > 0) {
            await Publish(contexts, cancellationToken);
        }
    }

    private async Task Publish(HashSet<UpdateProductTrackingContext> contexts, CancellationToken cancellationToken) {
        foreach (var context in contexts) {
            await this.bus.Publish(context, cancellationToken);
        }
    }
}
