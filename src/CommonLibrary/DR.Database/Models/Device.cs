namespace DR.Database.Models;

public partial class Device {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? StoreId { get; set; }
    public string? WarehouseId { get; set; }
    public bool IsActive { get; set; }

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedDate { get; set; } = DateTimeOffset.UtcNow;
}

internal class DeviceConfig : IEntityTypeConfiguration<Device> {

    public void Configure(EntityTypeBuilder<Device> builder) {
        builder.ToTable(nameof(Device));

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.SerialNumber).HasMaxLength(20).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(20).IsRequired();

        builder.Property(o => o.StoreId).HasMaxLength(32);
        builder.Property(o => o.WarehouseId).HasMaxLength(32);

        builder.Property(o => o.CreatedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();
        builder.Property(o => o.ModifiedDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

        // index
        builder.HasIndex(o => o.MerchantId);
        builder.HasIndex(o => new { o.MerchantId, o.SerialNumber }).IsUnique();
        builder.HasIndex(o => new { o.MerchantId, o.Name }).IsUnique();
        builder.HasIndex(o => new { o.MerchantId, o.SerialNumber, o.Name }).IsUnique();
    }
}
