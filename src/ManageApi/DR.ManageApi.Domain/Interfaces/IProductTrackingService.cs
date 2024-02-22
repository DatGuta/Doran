using DR.Common.Models;
using DR.Constant.Enums;

namespace DR.ManageApi.Domain.Services.Interfaces;
public interface IProductTrackingService {

    Task Save(string merchantId, string warehouseId,
        EProductDocumentType docType, string docId, string docCode,
        List<PtItem> items, CancellationToken cancellationToken);

    Task Remove(string merchantId, string warehouseId,
        EProductDocumentType docType, string docId, string docCode,
        List<PtItem> items, CancellationToken cancellationToken);
}
