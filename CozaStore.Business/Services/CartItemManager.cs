using CozaStore.Business.Contracts;
using CozaStore.Core.DataAccess;
using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.Services;

/// <summary>
/// Sepet öğeleriyle ilgili iş kurallarını yöneten servis.
/// </summary>
public class CartItemManager : ICartItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CartItem> _validator;

    public CartItemManager(IUnitOfWork unitOfWork, IValidator<CartItem> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<DataResult<IEnumerable<CartItem>>> GetByUserAsync(string userId)
    {
        var items = await _unitOfWork.CartItems.FindAsync(ci => ci.UserId == userId);
        return new SuccessDataResult<IEnumerable<CartItem>>(items);
    }

    public async Task<Result> AddAsync(CartItem cartItem)
    {
        await _validator.ValidateAndThrowAsync(cartItem);

        var existing = await _unitOfWork.CartItems.FindAsync(ci =>
            ci.UserId == cartItem.UserId && ci.ProductId == cartItem.ProductId);

        var existingItem = existing.FirstOrDefault();

        if (existingItem is not null)
        {
            existingItem.Quantity += cartItem.Quantity;
            await _unitOfWork.CartItems.UpdateAsync(existingItem);
        }
        else
        {
            await _unitOfWork.CartItems.AddAsync(cartItem);
        }

        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Sepete eklendi.");
    }

    public async Task<Result> UpdateQuantityAsync(Guid cartItemId, int quantity)
    {
        var item = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
        if (item is null)
            return new ErrorResult("Sepet öğesi bulunamadı.");

        if (quantity <= 0)
        {
            await _unitOfWork.CartItems.DeleteAsync(item);
        }
        else
        {
            item.Quantity = quantity;
            await _unitOfWork.CartItems.UpdateAsync(item);
        }

        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Sepet güncellendi.");
    }

    public async Task<Result> RemoveAsync(Guid cartItemId)
    {
        var item = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
        if (item is null)
            return new ErrorResult("Sepet öğesi bulunamadı.");

        await _unitOfWork.CartItems.DeleteAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Sepetten silindi.");
    }

    public async Task<Result> ClearAsync(string userId)
    {
        var items = await _unitOfWork.CartItems.FindAsync(ci => ci.UserId == userId);
        foreach (var item in items)
        {
            await _unitOfWork.CartItems.DeleteAsync(item);
        }

        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Sepet temizlendi.");
    }
}





