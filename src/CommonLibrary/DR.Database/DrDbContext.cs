using DR.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DR.Database;

public  class DrDbContext : DbContext {
    public DbSet<Merchant> Merchants => Set<Merchant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Permission> Permissions => Set<Permission>();
 
    public const string ConnectString = "Host=tmdsoft.vn;Port=5432;Database=doran;Username=postgres;Password=12345678x@X";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        // Mình sài postgree sql 
        optionsBuilder.UseNpgsql(ConnectString);
    }
}
