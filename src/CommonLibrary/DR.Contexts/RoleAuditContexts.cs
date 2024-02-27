using DR.Constant.Enums;
using DR.Database.Models;
using DR.Contexts;

namespace DR.Contexts {
    public class RoleCAuditContext : AuditContext {

        public RoleCAuditContext(Guid merchantId, string userId, Role entity)
            : base(merchantId, userId, EAuditAction.Create, EAuditDocType.Role, entity.Id, entity.Code) {
            Entity = entity;
        }

        public Role Entity { get; set; }
    }

    public class RoleUAuditContext : AuditContext {

        public RoleUAuditContext(Guid merchantId, string userId, Role original, Role actual)
            : base(merchantId, userId, EAuditAction.Update, EAuditDocType.Role, original.Id, original.Code) {
            Original = original;
            Actual = actual;
        }

        public Role Original { get; set; }
        public Role Actual { get; set; }
    }

    public class RoleDAuditContext : AuditContext {

        public RoleDAuditContext(Guid merchantId, string userId, Role entity)
            : base(merchantId, userId, EAuditAction.Delete, EAuditDocType.Role, entity.Id, entity.Code) {
            Entity = entity;
        }

        public Role Entity { get; set; }
    }
}
