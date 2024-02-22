using DR.Attributes;
using DR.Common.Extensions;
using DR.Constant.Enums;

namespace DR.ManageApi.Application.Handlers.CustomerHandlers.Queries;

public class DownloadTemplateImportCustomerQuery : IRequest<FileResult> {
    public EDownloadTemplate ExportFile { get; set; }
}

internal class DownloadTemplateImportCustomerHandler(IServiceProvider serviceProvider)
    : BaseHandler<DownloadTemplateImportCustomerQuery, FileResult>(serviceProvider) {
    // TODO: Thực hiện fix import customer and export template factory (Văn Toàn)
    public override async Task<FileResult> Handle(DownloadTemplateImportCustomerQuery request, CancellationToken cancellationToken) {
        var result = new FileResult() {
            FileName = $"import-customer-template{request.ExportFile.GetValue<FileExtensionAttribute>(o => o.Extension)}",
            ByteArray = [],//DownloadTemplateFactory.GetFile<ImportCustomerDto>(request.ExportFile),
        };
        return await Task.FromResult(result);
    }
}
