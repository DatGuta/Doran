
namespace DR.Handlers.AuthHandlers {
    public class PosCheckReq : BaseReq<bool> {
        public string StoreId { get; set; } = null!;
        public string WarehouseId { get; set; } = null!;
    }

    //public class PosCheckHandler : BaseHandler<PosCheckReq, bool> {

    //    public PosCheckHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
    //    }

    //    public override async Task<bool> Handle(PosCheckReq request, CancellationToken cancellationToken) {
    //        if (string.IsNullOrWhiteSpace(request.StoreId) || string.IsNullOrWhiteSpace(request.WarehouseId))
    //            return false;

    //        var validStore = await this.db.Stores.AsNoTracking()
    //            .Where(o => o.MerchantId == request.MerchantId && o.Id == request.StoreId && o.IsActive && !o.IsDelete)
    //            .Select(o => o.Id).AnyAsync(cancellationToken);
    //        if (!validStore) return false;

    //        var validWarehouse = await this.db.Warehouses.AsNoTracking()
    //            .Where(o => o.MerchantId == request.MerchantId && o.Id == request.WarehouseId && o.IsActive && !o.IsDelete)
    //            .Select(o => o.Id).AnyAsync(cancellationToken);

    //        return validWarehouse;
    //    }
    //}
}
