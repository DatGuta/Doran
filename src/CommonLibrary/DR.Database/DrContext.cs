using System.Reflection;
using DR.Database.Models;

namespace DR.Database;

public partial class DrContext : DbContext {
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Permission> Permissions => Set<Permission>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        // Mình sài postgree sql
        if (!optionsBuilder.IsConfigured) {
            optionsBuilder.UseNpgsql("Host=tmdsoft.vn;Port=5432;Database=doran;Username=postgres;Password=12345678x@X");
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
