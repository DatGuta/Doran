using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class Permission : ICloneable {
    public string Id { get; set; } = null!;
    public string? ParentId { get; set; }
    public string ClaimName { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public bool IsClaim { get; set; }
    public int OrderIndex { get; set; }
    public EPermission Type { get; set; }

    public virtual ICollection<RolePermission>? RolePermissions { get; set; }

    public object Clone() {
        return new Permission {
            Id = Id,
            ParentId = ParentId,
            ClaimName = ClaimName,
            DisplayName = DisplayName,
            IsDefault = IsDefault,
            IsActive = IsActive,
            IsClaim = IsClaim,
            OrderIndex = OrderIndex,
            Type = Type,
        };
    }
}

internal class PermissionConfig : IEntityTypeConfiguration<Permission> {

    public void Configure(EntityTypeBuilder<Permission> builder) {
        builder.ToTable(nameof(Permission));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.ParentId).HasMaxLength(32);

        builder.Property(o => o.ClaimName).HasMaxLength(50).IsRequired();
        builder.Property(o => o.DisplayName).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Type).HasMaxLength(20).IsRequired();

        // fk
        builder.HasMany(o => o.RolePermissions).WithOne(o => o.Permission).HasForeignKey(o => o.PermissionId);

        // seed data
        
    }
}
