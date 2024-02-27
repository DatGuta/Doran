using DR.Constant.Enums;
using DR.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TuanVu.Database.Models {
    public partial class MerchantSetting {
        public Guid? MerchantId { get; set; } = null!;
        public ESetting Code { get; set; }
        public JToken Value { get; set; } = default!;
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

        public virtual GeneralSetting? GeneralSetting { get; set; }
    }

    internal class MerchantSettingConfig : IEntityTypeConfiguration<MerchantSetting> {

        public void Configure(EntityTypeBuilder<MerchantSetting> builder) {
            builder.ToTable(nameof(MerchantSetting));

            builder.HasKey(o => new { o.MerchantId, o.Code });

            builder.Property(o => o.Code).HasMaxLength(100);
            builder.Property(o => o.Value).HasConversion(o => o.ToString(Formatting.None), o => JToken.Parse(o));
            builder.Property(o => o.Date).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            // fk
            builder.HasOne(o => o.GeneralSetting).WithMany(o => o.MerchantSettings).HasForeignKey(o => o.Code);
        }
    }
}
