using System.Diagnostics.CodeAnalysis;
using DR.Constant.Enums;

namespace DR.Models {
    public class AuditDto {
        public string Id { get; set; } = null!;
        public UserDto? User { get; set; }

        public EAuditAction Action { get; set; }

        public EAuditDocType DocType { get; set; }
        public string? DocId { get; set; }
        public string? DocCode { get; set; }

        public string? Title { get; set; }
        public string? Content { get; set; }

        public DateTimeOffset Time { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static AuditDto? FromEntity(Database.Models.UserAudit? entity) {
            if (entity == null) return default;

            return new AuditDto {
                Id = entity.Id,
                Action = entity.Action,
                DocType = entity.DocType,
                DocId = entity.DocId,
                DocCode = entity.DocCode,
                Title = entity.Title,
                Content = entity.Content,
                Time = entity.Time,
                User = UserDto.FromEntity(entity?.User, null),
            };
        }
    }
}
