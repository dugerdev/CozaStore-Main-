using CozaStore.Business.Contracts;
using CozaStore.Core.DataAccess;
using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.Services;

/// <summary>
/// Kullanıcı adresleri için iş kurallarını yöneten servis.
/// </summary>
public class AddressManager : IAddressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<Address> _validator;

    public AddressManager(IUnitOfWork unitOfWork, IValidator<Address> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<DataResult<Address>> GetByIdAsync(Guid id)
    {
        var address = await _unitOfWork.Addresses.GetByIdAsync(id);
        if (address is null)
            return new ErrorDataResult<Address>(null!, "Adres bulunamadı.");

        return new SuccessDataResult<Address>(address);
    }

    public async Task<DataResult<IEnumerable<Address>>> GetByUserAsync(string userId)
    {
        var addresses = await _unitOfWork.Addresses.FindAsync(a => a.UserId == userId);
        return new SuccessDataResult<IEnumerable<Address>>(addresses);
    }

    public async Task<Result> AddAsync(Address address)
    {
        await _validator.ValidateAndThrowAsync(address);

        await _unitOfWork.Addresses.AddAsync(address);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Adres eklendi.");
    }

    public async Task<Result> UpdateAsync(Address address)
    {
        await _validator.ValidateAndThrowAsync(address);

        await _unitOfWork.Addresses.UpdateAsync(address);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Adres güncellendi.");
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        await _unitOfWork.Addresses.SoftDeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Adres silindi.");
    }

    public async Task<Result> SetDefaultAsync(string userId, Guid addressId)
    {
        var addresses = await _unitOfWork.Addresses.FindAsync(a => a.UserId == userId);
        foreach (var address in addresses)
        {
            address.IsDefault = address.Id == addressId;
            await _unitOfWork.Addresses.UpdateAsync(address);
        }

        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Varsayılan adres güncellendi.");
    }
}





