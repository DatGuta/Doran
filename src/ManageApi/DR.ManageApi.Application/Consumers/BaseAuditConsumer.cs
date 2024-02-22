using DR.Constant.Enums;
using DR.Contexts;
using DR.Database.Models;
using DR.Helper;
using Markdown;

namespace DR.ManageApi.Application.Consumers;

internal class AuditContent<TAuditContext>(TAuditContext context, CancellationToken cancellationToken)
    where TAuditContext : AuditContext {
    public string MerchantId => Context.MerchantId;
    public string UserId => Context.UserId;
    public EAuditAction Action => Context.Action;
    public EAuditDocType DocType => Context.DocumentType;
    public string? DocId => Context.DocumentId;
    public string? DocCode => Context.DocumentCode;
    public DateTimeOffset Time => Context.Time;

    public string Title { get; set; } = string.Empty;
    public bool IsBuild { get; set; } = true;

    public TAuditContext Context { get; } = context;
    public IMarkdownDocument ContentBuilder { get; } = new MarkdownDocument();
    public CancellationToken CancellationToken { get; } = cancellationToken;
}

public abstract class BaseAuditConsumer<TAuditContext> : BaseConsumer<TAuditContext>
    where TAuditContext : AuditContext {

    protected BaseAuditConsumer(IServiceProvider serviceProvider) : base(serviceProvider) {
    }

    public override sealed async Task Handle(TAuditContext context, CancellationToken cancellationToken) {
        var auditContent = new AuditContent<TAuditContext>(context, cancellationToken);
        await this.BuildContent(auditContent);
        if (!auditContent.IsBuild) return;

        var audit = new UserAudit {
            Id = NGuidHelper.New(),
            MerchantId = auditContent.MerchantId,
            UserId = auditContent.UserId,
            Action = auditContent.Action,
            DocType = auditContent.DocType,
            DocId = auditContent.DocId,
            DocCode = auditContent.DocCode,
            Time = auditContent.Time,
            Title = auditContent.Title,
            Content = auditContent.ContentBuilder.ToString(),
        };

        await db.UserAudits.AddAsync(audit, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    internal abstract Task BuildContent(AuditContent<TAuditContext> content);
}
