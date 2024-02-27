

using DR.Constant.Enums;
using DR.Database.Models;
using DR.Contexts;

namespace DR.Contexts.AuditContexts {
    public class UserCAuditContext : AuditContext {

        public UserCAuditContext(Guid merchantId, string userId, User entity)
            : base(merchantId, userId, EAuditAction.Update, EAuditDocType.User, entity.Id, entity.Username) {
            this.Entity = entity;
        }

        public User Entity { get; set; }
    }

    public class UserUAuditContext : AuditContext {

        public UserUAuditContext(Guid merchantId, string userId, User original, User actual)
            : base(merchantId, userId, EAuditAction.Update, EAuditDocType.User, original.Id, original.Username) {
            this.Original = original;
            this.Actual = actual;
        }

        public User Original { get; set; }
        public User Actual { get; set; }
    }

    public class UserDAuditContext : AuditContext {

        public UserDAuditContext(Guid merchantId, string userId, User entity)
            : base(merchantId, userId, EAuditAction.Delete, EAuditDocType.User, entity.Id, entity.Username) {
            this.Entity = entity;
        }

        public User Entity { get; set; }
    }
}
