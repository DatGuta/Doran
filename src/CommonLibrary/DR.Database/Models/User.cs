namespace DR.Database.Models;

public partial class User : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;

    [Description("Phân quyền")]
    public string? RoleId { get; set; }

    [Description("Tên đăng nhập")]
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
    public string PinCode { get; set; } = null!;

    [Description("Tên người dùng")]
    public string Name { get; set; } = null!;

    public string SearchName { get; set; } = null!;

    [Description("Số điện thoại")]
    public string? Phone { get; set; }

    [Description("Tỉnh/Thành phố")]
    public string? Province { get; set; }

    [Description("Quận/Huyện")]
    public string? District { get; set; }

    [Description("Phường/Xã")]
    public string? Commune { get; set; }

    [Description("Địa chỉ")]
    public string? Address { get; set; }

    [Description("Trạng thái")]
    public bool IsActive { get; set; }

    public bool IsAdmin { get; set; }
    public bool IsSystem { get; set; }
    public bool IsDelete { get; set; }

    public long LastSession { get; set; }

    public virtual Role? Role { get; set; }
    public virtual ICollection<Order>? Orders { get; set; }
    public virtual ICollection<OrderPayment>? OrderPayments { get; set; }
    public virtual ICollection<WarehouseImport>? WarehouseImportCreateds { get; set; }
    public virtual ICollection<WarehouseImport>? WarehouseImportUpdateds { get; set; }
    public virtual ICollection<WarehouseExport>? WarehouseExportCreateds { get; set; }
    public virtual ICollection<WarehouseExport>? WarehouseExportUpdateds { get; set; }
    public virtual ICollection<WarehouseTransfer>? WarehouseTransferCreateds { get; set; }
    public virtual ICollection<WarehouseTransfer>? WarehouseTransferUpdateds { get; set; }
    public virtual ICollection<WarehouseRefund>? WarehouseRefundCreateds { get; set; }
    public virtual ICollection<WarehouseRefund>? WarehouseRefundUpdateds { get; set; }
    public virtual ICollection<WarehouseAdjustment>? WarehouseAdjustments { get; set; }
    public virtual ICollection<UserAudit>? UserAudits { get; set; }
    public virtual ICollection<File>? Files { get; set; }
    public virtual ICollection<Debt>? Debts { get; set; }
}

internal class UserConfig : IEntityTypeConfiguration<User> {

    public void Configure(EntityTypeBuilder<User> builder) {
        builder.ToTable(nameof(User));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();
        builder.Property(o => o.RoleId).HasMaxLength(32);

        builder.Property(o => o.Username).IsRequired();
        builder.Property(o => o.Password).IsRequired();
        builder.Property(o => o.PinCode).IsRequired();

        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.SearchName).HasMaxLength(255).IsRequired();

        builder.Property(o => o.Province).HasMaxLength(20);
        builder.Property(o => o.District).HasMaxLength(20);
        builder.Property(o => o.Commune).HasMaxLength(20);
        builder.Property(o => o.Address).HasMaxLength(255);

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.Username }).IsUnique();

        // fk
        builder.HasOne(o => o.Role).WithMany(o => o.Users).HasForeignKey(o => o.RoleId);

        builder.HasMany(o => o.Orders).WithOne(o => o.User).HasForeignKey(o => o.CreatedBy);
        builder.HasMany(o => o.OrderPayments).WithOne(o => o.CreatedUser).HasForeignKey(o => o.CreatedBy);
        builder.HasMany(o => o.WarehouseImportCreateds).WithOne(o => o.CreatedUser).HasForeignKey(o => o.CreatedBy);
        builder.HasMany(o => o.WarehouseImportUpdateds).WithOne(o => o.UpdatedUser).HasForeignKey(o => o.LastUpdateBy);
        builder.HasMany(o => o.WarehouseExportCreateds).WithOne(o => o.CreatedUser).HasForeignKey(o => o.CreatedBy);
        builder.HasMany(o => o.WarehouseExportUpdateds).WithOne(o => o.UpdatedUser).HasForeignKey(o => o.LastUpdateBy);
        builder.HasMany(o => o.WarehouseRefundCreateds).WithOne(o => o.CreatedUser).HasForeignKey(o => o.CreatedBy);
        builder.HasMany(o => o.WarehouseRefundUpdateds).WithOne(o => o.UpdatedUser).HasForeignKey(o => o.LastUpdateBy);
        builder.HasMany(o => o.WarehouseTransferCreateds).WithOne(o => o.CreatedUser).HasForeignKey(o => o.CreatedBy);
        builder.HasMany(o => o.WarehouseTransferUpdateds).WithOne(o => o.UpdatedUser).HasForeignKey(o => o.LastUpdateBy);
        builder.HasMany(o => o.WarehouseAdjustments).WithOne(o => o.User).HasForeignKey(o => o.CreatedBy);
        builder.HasMany(o => o.UserAudits).WithOne(o => o.User).HasForeignKey(o => o.UserId);
        builder.HasMany(o => o.Files).WithOne(o => o.User).HasForeignKey(o => o.UploadBy);
        builder.HasMany(o => o.Debts).WithOne(o => o.User).HasForeignKey(o => o.UserId);
    }
}
