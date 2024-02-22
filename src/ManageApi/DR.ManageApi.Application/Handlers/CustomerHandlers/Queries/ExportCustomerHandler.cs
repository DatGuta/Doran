using DR.ManageApi.Application.Handlers;
using DR.Resource;

namespace DR.ManageApi.Application.Handlers.CustomerHandlers.Queries;

public class ExportCustomerReq : PaginatedRequest<FileResult> {
}

internal class ExportCustomerHandler(IServiceProvider serviceProvider) : BaseHandler<ExportCustomerReq, FileResult>(serviceProvider) {
    private readonly UnitResource unitRes = serviceProvider.GetRequiredService<UnitResource>();


    // TODO: implement export customer
    public override async Task<FileResult> Handle(ExportCustomerReq request, CancellationToken cancellationToken) {
        //var data = new List<CustomerExportData>();

        //var query = this.db.Customers.AsNoTracking().Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
        //  .WhereFunc(!string.IsNullOrWhiteSpace(request.SearchText), q => {
        //      var searchText = StringHelper.UnsignedUnicode(request.SearchText!);
        //      var searchPhone = PhoneHasher.Encrypt(searchText);
        //      return q.Where(o => o.Code.Contains(searchText!.ToUpper())
        //          || o.SearchName.Contains(searchText)
        //          || (o.CustomerPhones != null && o.CustomerPhones.Any(x => x.SearchPhone == searchPhone || x.SearchLastPhone == searchPhone)));
        //  });

        //var customers = await query.OrderBy(o => o.Code).ToListAsync(cancellationToken);

        //int count = 1;
        //foreach (var customer in customers) {
        //    data.Add(new CustomerExportData() {
        //        No = count++,
        //        Code = customer.Code,
        //        Name = customer.Name,
        //        Phone = PhoneHasher.Decrypt(customer.Phone),
        //        Province = this.unitRes.GetByCode(customer.Province)?.Name,
        //        District = this.unitRes.GetByCode(customer.District)?.Name,
        //        Commune = this.unitRes.GetByCode(customer.Commune)?.Name,
        //        Address = customer.Address
        //    });
        //}

        return await Task.FromResult(
            new FileResult {
                FileName = $"DanhSachKhachHang-{DateTime.Now:dd/MM/yyyy}.xlsx",
                ByteArray = []//CustomerExporter.Export(data, EExportFile.Excel),
            }
        );
    }
}
