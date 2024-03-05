using DR.Constant.Enums;

namespace DR.Database.Models;

public partial class Permission : ICloneable {
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
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
        var index = 0;
        builder.HasData(new Permission {
            Id = Guid.Parse("ec0f270b424249438540a16e9157c0c8"),
            ClaimName = "DR",
            DisplayName = "Trang quản lý",
            IsDefault = true,
            IsActive = true,
            IsClaim = false,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = Guid.Parse("b47bbb68c29e4880bb3a230620ce4e6e"),
            ParentId = Guid.Parse("ec0f270b424249438540a16e9157c0c8"),
            ClaimName = "DR.Dashboard",
            DisplayName = "Thông kê",
            IsDefault = true,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        // Tab cài đặt
        builder.HasData(new Permission {
            Id = Guid.Parse("dc1c2ce584d74428b4e5241a5502787d"),
            ParentId = Guid.Parse("ec0f270b424249438540a16e9157c0c8"),
            ClaimName = "DR.Setting",
            DisplayName = "Cài đặt",
            IsDefault = false,
            IsActive = true,
            IsClaim = false,
            OrderIndex = index++,
            Type = EPermission.Web,
        });


        // Người đùng
        builder.HasData(new Permission {
            Id = Guid.Parse("296285809bac481890a454ea8aed6af4"),
            ParentId = Guid.Parse("dc1c2ce584d74428b4e5241a5502787d"),
            ClaimName = "DR.User",
            DisplayName = "Người dùng",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        }, new Permission { // Con người dùng
            Id = Guid.Parse("98873832ebcb4d9fb12e9b21a187f12c"),
            ParentId = Guid.Parse("296285809bac481890a454ea8aed6af4"),
            ClaimName = "DR.User.Reset",
            DisplayName = "Đặt lại mật khẩu",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });

        builder.HasData(new Permission {
            Id = Guid.Parse("cb26c94262ab4863baa6c516edfde134"),
            ParentId = Guid.Parse("dc1c2ce584d74428b4e5241a5502787d"),
            ClaimName = "DR.Role",
            DisplayName = "Phân quyền",
            IsDefault = false,
            IsActive = true,
            IsClaim = true,
            OrderIndex = index++,
            Type = EPermission.Web,
        });
    }
}
