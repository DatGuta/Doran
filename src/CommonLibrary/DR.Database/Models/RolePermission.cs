namespace DR.Database.Models;

public partial class RolePermission {
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public bool IsEnable { get; set; }

    public virtual Role? Role { get; set; }
    public virtual Permission? Permission { get; set; }
}

internal class RolePermissionConfig : IEntityTypeConfiguration<RolePermission> {

    public void Configure(EntityTypeBuilder<RolePermission> builder) {
        builder.ToTable(nameof(RolePermission));

        builder.HasKey(o => new { o.RoleId, o.PermissionId });

        // fk
        builder.HasOne(o => o.Role).WithMany(o => o.RolePermissions).HasForeignKey(o => o.RoleId);
        builder.HasOne(o => o.Permission).WithMany(o => o.RolePermissions).HasForeignKey(o => o.PermissionId);
    }
}
