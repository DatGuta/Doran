

namespace DR.Database.Models;
public partial class User {

    public Guid Id { get; set; }

    [Description("Phân quyền")]
    public Guid? RoleId { get; set; }

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

    public bool IsAdmin { get; set; }
    public bool IsSystem { get; set; }
    public bool IsDelete { get; set; }

    public long LastSession { get; set; }

    public virtual Role? Role { get; set; }
}
internal class UserConfig : IEntityTypeConfiguration<User> {

    public void Configure(EntityTypeBuilder<User> builder) {
        builder.ToTable(nameof(User));

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Username).IsRequired();
        builder.Property(o => o.Password).IsRequired();
        builder.Property(o => o.PinCode).IsRequired();

        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.SearchName).HasMaxLength(255).IsRequired();

        builder.Property(o => o.Province).HasMaxLength(20);
        builder.Property(o => o.District).HasMaxLength(20);
        builder.Property(o => o.Commune).HasMaxLength(20);
        builder.Property(o => o.Address).HasMaxLength(255);

        // fk
        builder.HasOne(o => o.Role).WithMany(o => o.Users).HasForeignKey(o => o.RoleId);
    }
}

