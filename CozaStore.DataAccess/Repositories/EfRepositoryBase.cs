using System.Linq.Expressions;
using CozaStore.Core.DataAccess;        // IRepository sözleşmesi
using CozaStore.Entities.Common;       // BaseEntity ortak alanları (Id, IsDeleted vb.)
using Microsoft.EntityFrameworkCore;   // DbContext ve DbSet API'leri

namespace CozaStore.DataAccess.Repositories;

/// <summary>
/// Entity Framework Core tabanlı generic repository implementasyonu.
/// Tüm entity tiplerinin ortak CRUD davranışını burada topluyoruz.
/// </summary>
/// <typeparam name="T">BaseEntity'den türeyen herhangi bir entity.</typeparam>
public class EfRepositoryBase<T> : IRepository<T> where T : BaseEntity
{
    private readonly DbContext _context;   // SaveChanges / transaction gibi işlemler için gerekiyor
    protected readonly DbSet<T> _dbSet;    // EF Core'un ilgili tabloya erişim noktası

    public EfRepositoryBase(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();         // EF Core entity tipine göre doğru DbSet'i verir
    }

    /// <summary>Primary key'e göre tek kayıt döndürür. Soft delete edilmiş olanları dışarıda tutar.</summary>
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(
            entity => entity.Id == id && !entity.IsDeleted,
            cancellationToken);

    /// <summary>Tüm kayıtları (IsDeleted = false) liste olarak döndürür.</summary>
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(entity => !entity.IsDeleted)
                       .ToListAsync(cancellationToken);

    /// <summary>Verilen koşula göre filtrelenmiş kayıt listesini döndürür.</summary>
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.Where(entity => !entity.IsDeleted)
                       .Where(predicate)
                       .ToListAsync(cancellationToken);

    /// <summary>Yeni bir kayıt ekler; kalıcı olması için UnitOfWork.SaveChangesAsync çağrılmalıdır.</summary>
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>Birden fazla kaydı toplu ekler.</summary>
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => await _dbSet.AddRangeAsync(entities, cancellationToken);

    /// <summary>Mevcut kaydı günceller; SaveChanges ile kalıcı olur.</summary>
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <summary>Fiziksel silme operasyonu. Soft delete için aşağıdaki metodu kullan.</summary>
    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>Birden fazla kaydı toplu siler.</summary>
    public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Kayıt soft delete edilir; IsDeleted, IsActive ve DeletedDate alanları güncellenir.
    /// </summary>
    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // GetByIdAsync IsDeleted filtresi yapar, bu yüzden direkt DbSet'ten alıyoruz
        var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is null) return;              // Kayıt bulunmazsa işlem yapılmaz
        
        // Zaten silinmişse tekrar silme
        if (entity.IsDeleted) return;

        entity.IsDeleted = true;                 // Soft delete işaretini ata
        entity.IsActive = false;                 // Artık aktif değil
        entity.DeletedDate = DateTime.UtcNow;    // Silinme zamanını kaydet

        _dbSet.Update(entity);                   // ChangeTracker değişikliği gözlemler
    }

    /// <summary>Belirtilen id'ye sahip (soft delete edilmemiş) kayıt var mı?</summary>
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(entity => entity.Id == id && !entity.IsDeleted, cancellationToken);
}
