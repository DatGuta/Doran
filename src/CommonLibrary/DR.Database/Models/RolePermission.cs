namespace DR.Database.Models;

public partial class RolePermission {
    public string RoleId { get; set; } = null!;
    public string PermissionId { get; set; } = null!;
    public bool IsEnable { get; set; }

    public virtual Role? Role { get; set; }
    public virtual Permission? Permission { get; set; }
}

internal class RolePermissionConfig : IEntityTypeConfiguration<RolePermission> {

    public void Configure(EntityTypeBuilder<RolePermission> builder) {
        builder.ToTable(nameof(RolePermission));

        builder.HasKey(o => new { o.RoleId, o.PermissionId });  
        builder.Property(o => o.RoleId).HasMaxLength(32);
        builder.Property(o => o.PermissionId).HasMaxLength(32);

        // fk
        builder.HasOne(o => o.Role).WithMany(o => o.RolePermissions).HasForeignKey(o => o.RoleId);
        builder.HasOne(o => o.Permission).WithMany(o => o.RolePermissions).HasForeignKey(o => o.PermissionId);
    }
}
