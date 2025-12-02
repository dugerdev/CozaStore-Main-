using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;

namespace CozaStore.Business.Contracts;

/// <summary>
/// İletişim mesajları için servis sözleşmesi.
/// </summary>
public interface IContactService
{
    /// <summary>
    /// Yeni bir iletişim mesajı ekler.
    /// </summary>
    Task<Result> SendMessageAsync(string email, string message);

    /// <summary>
    /// Tüm iletişim mesajlarını getirir.
    /// </summary>
    Task<DataResult<IEnumerable<Contact>>> GetAllAsync();

    /// <summary>
    /// Okunmamış mesajları getirir.
    /// </summary>
    Task<DataResult<IEnumerable<Contact>>> GetUnreadAsync();

    /// <summary>
    /// Mesajı okundu olarak işaretler.
    /// </summary>
    Task<Result> MarkAsReadAsync(Guid contactId);

    /// <summary>
    /// ID'ye göre iletişim mesajını getirir.
    /// </summary>
    Task<DataResult<Contact>> GetByIdAsync(Guid contactId);

    /// <summary>
    /// İletişim mesajını siler.
    /// </summary>
    Task<Result> DeleteAsync(Guid contactId);
}


