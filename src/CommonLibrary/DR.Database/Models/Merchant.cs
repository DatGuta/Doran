using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DR.Database.Models;
public class Merchant {
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
internal class MerchantConfig : IEntityTypeConfiguration<Merchant> {

    public void Configure(EntityTypeBuilder<Merchant> builder) {
        builder.ToTable(nameof(Merchant));

        builder.HasKey(o => o.Id);
        builder.Property(O => O.Name).HasMaxLength(36).IsRequired();
        //index
    }
}
