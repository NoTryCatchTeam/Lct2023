using Lct2023.Api.Definitions.Constants;
using Lct2023.Api.Definitions.Entities;
using Lct2023.Api.Definitions.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Dal;

public class NpgsqlDbContext : IdentityDbContext<ExtendedIdentityUser, IdentityRole<int>, int>
{
    private readonly string _connectionString;

    public NpgsqlDbContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetValue<string>(ConfigurationConstants.Secrets.DB_CONNECTION_STRING)!;
    }

    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options
            .UseNpgsql(_connectionString)
            .LogTo(Console.WriteLine);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ExtendedIdentityUser>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("NOW()");
        
        modelBuilder.Entity<ExtendedIdentityUser>()
            .HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User);
    }
}
