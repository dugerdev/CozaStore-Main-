using CozaStore.Entities.Entities;
using CozaStore.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace CozaStore.DataAccess.Data;

/// <summary>
/// EF Core DbContext sınıfı. Uygulamanın veritabanı bağlamını temsil eder.
/// </summary>
public class CozaStoreDbContext : IdentityDbContext<ApplicationUser, ApplicationRole,Guid,ApplicationUserClaim,ApplicationUserRole,
    ApplicationUserLogin,ApplicationRoleClaim,ApplicationUserToken>
{
    public CozaStoreDbContext(DbContextOptions<CozaStoreDbContext> options) : base(options)
    {
    }

    // DbSet tanımları - her entity için bir tabloyu temsil eder
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<WishList> WishLists { get; set; } = null!;
    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<BlogPost> BlogPosts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(b => b.ToTable("Users"));
        modelBuilder.Entity<ApplicationRole>(b => b.ToTable("Roles"));
        modelBuilder.Entity<ApplicationUserRole>(b => b.ToTable("UserRoles"));
        modelBuilder.Entity<ApplicationUserClaim>(b => b.ToTable("UserClaims"));
        modelBuilder.Entity<ApplicationRoleClaim>(b => b.ToTable("RoleClaims"));
        modelBuilder.Entity<ApplicationUserToken>(b => b.ToTable("UserTokens"));
        modelBuilder.Entity<ApplicationUserLogin>(b => b.ToTable("UserLogins"));


        // Tüm IEntityTypeConfiguration implementasyonlarını assembly üzerinden uygular
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CozaStoreDbContext).Assembly);
    }
}



