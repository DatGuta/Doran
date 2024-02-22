
using MediatR;

namespace DR.Contexts;

public class AuditContext : INotification {
    public string MerchantId { get; set; }
    public string UserId { get; set; }
    public EAuditAction Action { get; set; }
    public EAuditDocType DocumentType { get; set; }
    public string DocumentId { get; set; }
    public string DocumentCode { get; set; }
    public DateTimeOffset Time { get; protected set; } = DateTimeOffset.UtcNow;

    public AuditContext(string merchantId, string userId, EAuditAction action, EAuditDocType documentType, string documentId, string documentCode) {
        this.MerchantId = merchantId;
        this.UserId = userId;
        this.Action = action;
        this.DocumentType = documentType;
        this.DocumentId = documentId;
        this.DocumentCode = documentCode;
    }
}
