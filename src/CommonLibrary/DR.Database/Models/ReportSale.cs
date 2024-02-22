namespace DR.Database.Models;

public class ReportSale : IEntity {
    public string Id { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public DateTimeOffset Date { get; set; }
    public long CountOrder { get; set; }
    public decimal Total { get; set; }
}

internal class SaleReportConfig : IEntityTypeConfiguration<ReportSale> {

    public void Configure(EntityTypeBuilder<ReportSale> builder) {
        builder.ToTable("Sale", DrSchema.Report);

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(32);
        builder.Property(o => o.MerchantId).HasMaxLength(32).IsRequired();

        builder.Property(o => o.Date).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o));
        // index
        builder.HasIndex(o => o.MerchantId);
    }
}
