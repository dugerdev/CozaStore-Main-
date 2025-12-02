using CozaStore.Core.DataAccess;           // IUnitOfWork sözleşmesini kullanmak için
using CozaStore.Entities.Common;           // BaseEntity (Id, IsDeleted vb.)
using CozaStore.Entities.Entities;         // Product, Category gibi entity tipleri
using Microsoft.EntityFrameworkCore;       // DbContext API’si
using Microsoft.EntityFrameworkCore.Storage;
using CozaStore.DataAccess.Data;

namespace CozaStore.DataAccess.Repositories;

/// <summary>
/// Repository örneklerini tek merkezden yöneten ve transaction / SaveChanges yönetimini üstlenen sınıf.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CozaStoreDbContext _context;           // EF Core context’i (CozaStoreDbContext enjekte edilecek)
    private IDbContextTransaction? _transaction;             // Aktif transaction referansı (varsa)
    private readonly Dictionary<Type, object> _repositories = new(); // Repository örneklerini cache’lemek için

    public UnitOfWork(CozaStoreDbContext context)
    {
        _context = context;
    }

    // Her property ilk çağrıldığında EfRepositoryBase<T> örneği oluşturup cache'e ekler.
    public IRepository<Product> Products => GetRepository<Product>();
    public IRepository<Category> Categories => GetRepository<Category>();
    public IRepository<Order> Orders => GetRepository<Order>();
    public IRepository<OrderDetail> OrderDetails => GetRepository<OrderDetail>();
    public IRepository<CartItem> CartItems => GetRepository<CartItem>();
    public IRepository<Address> Addresses => GetRepository<Address>();
    public IRepository<Review> Reviews => GetRepository<Review>();
    public IRepository<WishList> WishLists => GetRepository<WishList>();
    public IRepository<Contact> Contacts => GetRepository<Contact>();
    public IRepository<BlogPost> BlogPosts => GetRepository<BlogPost>();

    /// <summary>
    /// EF Core’un SaveChangesAsync metodunu sarar; yapılan tüm değişiklikleri kalıcı hale getirir.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    /// <summary>
    /// Transaction başlatır. Zaten aktif bir transaction varsa yeni bir tane açılmaz.
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null) return;
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Transaction’ı commit eder ve kaynağı serbest bırakır.
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) return;

        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    /// <summary>
    /// Transaction’ı rollback eder; herhangi bir hata durumunda geri dönüş sağlar.
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) return;

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    /// <summary>
    /// UnitOfWork yaşam döngüsü bittiğinde transaction ve DbContext’i temizler.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
            await _transaction.DisposeAsync();

        await _context.DisposeAsync();
    }

    /// <summary>
    /// Generic repository örneğini lazily oluşturur ve sözlükte cache’ler.
    /// Böylece aynı entity için tekrar tekrar nesne üretilmez.
    /// </summary>
    private IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
    {
        var type = typeof(TEntity);

        if (!_repositories.TryGetValue(type, out var repository))
        {
            repository = new EfRepositoryBase<TEntity>(_context); // İlk defa isteniyorsa oluştur
            _repositories[type] = repository;                      // Sözlüğe ekle
        }

        return (IRepository<TEntity>)repository;
    }
}



