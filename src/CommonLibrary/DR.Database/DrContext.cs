using System.Reflection;
using DR.Database.Models;

namespace DR.Database {

    public partial class DrContext : DbContext {

        // system
        public virtual DbSet<Permission> Permissions => Set<Permission>();

        // merchant

        public virtual DbSet<Merchant> Merchants => Set<Merchant>();
        public virtual DbSet<MerchantItemDefault> MerchantItemDefaults => Set<MerchantItemDefault>();

        public virtual DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public virtual DbSet<Store> Stores => Set<Store>();

        // user
        public virtual DbSet<User> Users => Set<User>();

        public virtual DbSet<UserAudit> UserAudits => Set<UserAudit>();
        public virtual DbSet<Role> Roles => Set<Role>();
        public virtual DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        // customer

        public virtual DbSet<Customer> Customers => Set<Customer>();
        public virtual DbSet<CustomerPhone> CustomerPhones => Set<CustomerPhone>();
        public virtual DbSet<CustomerDebt> CustomerDebts => Set<CustomerDebt>();
        public virtual DbSet<CustomerTracking> CustomerTrackings => Set<CustomerTracking>();
        public virtual DbSet<CustomerGroup> CustomerGroups => Set<CustomerGroup>();

        //debt

        public virtual DbSet<Debt> Debts => Set<Debt>();
        public virtual DbSet<DebtDetail> DebtDetails => Set<DebtDetail>();

        // product

        public virtual DbSet<Product> Products => Set<Product>();
        public virtual DbSet<ProductOnStore> ProductOnStores => Set<ProductOnStore>();
        public virtual DbSet<ProductOnWarehouse> ProductOnWarehouses => Set<ProductOnWarehouse>();
        public virtual DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public virtual DbSet<ProductSupplier> ProductSuppliers => Set<ProductSupplier>();
        public virtual DbSet<ProductBrand> ProductBrands => Set<ProductBrand>();

        // supplier
        public virtual DbSet<Supplier> Suppliers => Set<Supplier>();

        // brand
        public virtual DbSet<Brand> Brands => Set<Brand>();

        // order

        public virtual DbSet<Order> Orders => Set<Order>();
        public virtual DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public virtual DbSet<OrderAction> OrderActions => Set<OrderAction>();
        public virtual DbSet<OrderActionDetail> OrderActionDetails => Set<OrderActionDetail>();
        public virtual DbSet<OrderPayment> OrderPayments => Set<OrderPayment>();
        public virtual DbSet<OrderCustomer> OrderCustomers => Set<OrderCustomer>();

        // image
        public virtual DbSet<ItemImage> ItemImages => Set<ItemImage>();

        // tracking
        public virtual DbSet<ProductTracking> ProductTrackings => Set<ProductTracking>();

        // setting
        public virtual DbSet<GeneralSetting> GeneralSettings => Set<GeneralSetting>();

        public virtual DbSet<MerchantSetting> MerchantSettings => Set<MerchantSetting>();

        // warehouseImport
        public virtual DbSet<WarehouseImport> WarehouseImports => Set<WarehouseImport>();

        public virtual DbSet<WarehouseImportDetail> WarehouseImportDetails => Set<WarehouseImportDetail>();

        // warehouseExport
        public virtual DbSet<WarehouseExport> WarehouseExports => Set<WarehouseExport>();

        public virtual DbSet<WarehouseExportDetail> WarehouseExportDetails => Set<WarehouseExportDetail>();

        // warehouseExportOther
        public virtual DbSet<WarehouseExportOther> WarehouseExportOthers => Set<WarehouseExportOther>();

        public virtual DbSet<WarehouseExportOtherDetail> WarehouseExportOtherDetails => Set<WarehouseExportOtherDetail>();

        // warehouseTransfer
        public virtual DbSet<WarehouseTransfer> WarehouseTransfers => Set<WarehouseTransfer>();

        public virtual DbSet<WarehouseTransferDetail> WarehouseTransferDetails => Set<WarehouseTransferDetail>();

        // warehouseRefund
        public virtual DbSet<WarehouseRefund> WarehouseRefunds => Set<WarehouseRefund>();

        public virtual DbSet<WarehouseRefundDetail> WarehouseRefundDetails => Set<WarehouseRefundDetail>();

        // warehouseAdjustment
        public virtual DbSet<WarehouseAdjustment> WarehouseAdjustments => Set<WarehouseAdjustment>();

        public virtual DbSet<WarehouseAdjustmentDetail> WarehouseAdjustmentDetails => Set<WarehouseAdjustmentDetail>();

        // category
        public virtual DbSet<Category> Categories => Set<Category>();

        // payment method
        public virtual DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

        // Dashboard
        public virtual DbSet<ReportSale> ReportSales => Set<ReportSale>();

        // File
        public virtual DbSet<Database.Models.File> Files => Set<Database.Models.File>();

        // Sync
        public virtual DbSet<SyncSession> SyncSessions => Set<SyncSession>();

        public virtual DbSet<SyncSessionItem> SyncSessionItems => Set<SyncSessionItem>();

        // Receipt
        public virtual DbSet<Receipt> Receipts => Set<Receipt>();

        public virtual DbSet<ReceiptDetail> ReceiptDetails => Set<ReceiptDetail>();

        // Payment
        public virtual DbSet<Payment> Payments => Set<Payment>();

        public virtual DbSet<PaymentDetail> PaymentDetails => Set<PaymentDetail>();

        // Device
        public virtual DbSet<Device> Devices => Set<Device>();

        public DrContext() {
        }

        public DrContext(string connectionString) : base(GetOptions(connectionString)) {
        }

        public DrContext(DbContextOptions<DrContext> options) : base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseNpgsql("User ID=tuanvudev;Password=tuanvudev;Server=db.tvfersfc.com;Port=21427;Database=tuanvu_dev;Integrated Security=true;Pooling=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema(DrSchema.Default);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private static DbContextOptions GetOptions(string connectionString) {
            return NpgsqlDbContextOptionsBuilderExtensions.UseNpgsql(new DbContextOptionsBuilder(), connectionString).Options;
        }
    }
}
