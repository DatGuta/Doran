using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DR.Model;

internal class WebContext : DbContext {
    public DbSet<Users> Users { get; set; }
    public DbSet<Profiles> Profiles { get; set; }

    public const string ConnectString = @"Data Source=localhost,1433;Initial Catalog=ProjectManageDR;User ID=SA;Password=wtsdatho11022000";

    public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
        builder
        .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Warning)
        .AddFilter(DbLoggerCategory.Query.Name, LogLevel.Debug)
        .AddConsole();
    }
    );

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(ConnectString);
        optionsBuilder.UseLoggerFactory(loggerFactory);
    }
}
