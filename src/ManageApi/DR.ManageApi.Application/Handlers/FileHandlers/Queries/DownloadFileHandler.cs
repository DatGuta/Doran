using DR.Common.Exceptions;
using DR.Helper;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.FileHandlers.Queries;

public class DownloadFileQuery : SingleRequest<FileResult> { }

internal class DownloadFileHandler(IServiceProvider serviceProvider) : BaseHandler<DownloadFileQuery, FileResult>(serviceProvider) {

    public override async Task<FileResult> Handle(DownloadFileQuery request, CancellationToken cancellationToken) {
        var file = await db.Files.FirstOrDefaultAsync(o => o.MerchantId == request.MerchantId && o.Id == request.Id, cancellationToken);
        ManagedException.ThrowIfNull(file, Messages.File.File_NotFound);
        var data = await FtpHelper.DownloadBytes(file.Path, configuration);
        return new() { ByteArray = data, FileName = file.Name };
    }
}
