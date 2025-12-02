using CozaStore.Business.Contracts;
using CozaStore.Core.DataAccess;
using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.Services;

/// <summary>
/// İletişim mesajları için iş kurallarını yöneten servis.
/// </summary>
public class ContactManager : IContactService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<Contact> _validator;

    public ContactManager(IUnitOfWork unitOfWork, IValidator<Contact> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result> SendMessageAsync(string email, string message)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
        {
            return new ErrorResult("Email ve mesaj alanları zorunludur.");
        }

        var contact = new Contact
        {
            Email = email.Trim(),
            Message = message.Trim(),
            IsRead = false
        };

        await _validator.ValidateAndThrowAsync(contact);

        await _unitOfWork.Contacts.AddAsync(contact);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız.");
    }

    public async Task<DataResult<IEnumerable<Contact>>> GetAllAsync()
    {
        var contacts = await _unitOfWork.Contacts.GetAllAsync();
        return new SuccessDataResult<IEnumerable<Contact>>(contacts);
    }

    public async Task<DataResult<IEnumerable<Contact>>> GetUnreadAsync()
    {
        var contacts = await _unitOfWork.Contacts.FindAsync(c => !c.IsRead);
        return new SuccessDataResult<IEnumerable<Contact>>(contacts);
    }

    public async Task<Result> MarkAsReadAsync(Guid contactId)
    {
        var contact = await _unitOfWork.Contacts.GetByIdAsync(contactId);
        if (contact is null)
            return new ErrorResult("Mesaj bulunamadı.");

        contact.IsRead = true;
        contact.ReadDate = DateTime.UtcNow;
        await _unitOfWork.Contacts.UpdateAsync(contact);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Mesaj okundu olarak işaretlendi.");
    }

    public async Task<DataResult<Contact>> GetByIdAsync(Guid contactId)
    {
        var contact = await _unitOfWork.Contacts.GetByIdAsync(contactId);
        if (contact is null)
            return new ErrorDataResult<Contact>(null!, "Mesaj bulunamadı.");

        return new SuccessDataResult<Contact>(contact);
    }

    public async Task<Result> DeleteAsync(Guid contactId)
    {
        var contact = await _unitOfWork.Contacts.GetByIdAsync(contactId);
        if (contact is null)
            return new ErrorResult("Mesaj bulunamadı.");

        await _unitOfWork.Contacts.DeleteAsync(contact);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Mesaj başarıyla silindi.");
    }
}


