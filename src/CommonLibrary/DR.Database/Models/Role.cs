namespace DR.Database.Models;

public partial class Role : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Mã phân quyền")]
    public string Code { get; set; } = null!;

    [Description("Tên phân quyền")]
    public string Name { get; set; } = null!;

    public string SearchName { get; set; } = null!;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public bool IsDelete { get; set; }

    public virtual ICollection<User>? Users { get; set; }
    public virtual ICollection<RolePermission>? RolePermissions { get; set; }
}

internal class RoleConfig : IEntityTypeConfiguration<Role> {

    public void Configure(EntityTypeBuilder<Role> builder) {
        builder.ToTable(nameof(Role));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.SearchName).HasMaxLength(255).IsRequired();
        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Code }).IsUnique();

        // fk
        builder.HasMany(o => o.Users).WithOne(o => o.Role).HasForeignKey(o => o.RoleId);
        builder.HasMany(o => o.RolePermissions).WithOne(o => o.Role).HasForeignKey(o => o.RoleId);
    }
}
