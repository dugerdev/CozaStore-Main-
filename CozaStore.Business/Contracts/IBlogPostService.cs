using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;

namespace CozaStore.Business.Contracts;

/// <summary>
/// Blog gönderileri için servis sözleşmesi.
/// </summary>
public interface IBlogPostService
{
    /// <summary>
    /// ID'ye göre blog gönderisini getirir.
    /// </summary>
    Task<DataResult<BlogPost>> GetByIdAsync(Guid id);

    /// <summary>
    /// Tüm blog gönderilerini getirir.
    /// </summary>
    Task<DataResult<IEnumerable<BlogPost>>> GetAllAsync();

    /// <summary>
    /// Yayınlanmış blog gönderilerini getirir.
    /// </summary>
    Task<DataResult<IEnumerable<BlogPost>>> GetPublishedAsync();

    /// <summary>
    /// Yeni bir blog gönderisi ekler.
    /// </summary>
    Task<Result> AddAsync(BlogPost blogPost);

    /// <summary>
    /// Blog gönderisini günceller.
    /// </summary>
    Task<Result> UpdateAsync(BlogPost blogPost);

    /// <summary>
    /// Blog gönderisini siler (soft delete).
    /// </summary>
    Task<Result> DeleteAsync(Guid id);
}

