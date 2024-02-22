using DR.Common.Exceptions;
using DR.Helper;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.FileHandlers.Commands;

public class DeleteFileCommand : SingleRequest { }

internal class DeleteFileHandler(IServiceProvider serviceProvider) : BaseHandler<DeleteFileCommand>(serviceProvider) {

    public override async Task Handle(DeleteFileCommand request, CancellationToken cancellationToken) {
        var entity = await db.Files.FirstOrDefaultAsync(o => o.MerchantId == request.MerchantId && o.Id == request.Id, cancellationToken);
        ManagedException.ThrowIf(entity == null, Messages.File.File_NotFound);

        await FtpHelper.DeleteFile(entity.Path, configuration);

        db.Files.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }
}
