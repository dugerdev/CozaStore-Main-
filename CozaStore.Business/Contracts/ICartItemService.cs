using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;

namespace CozaStore.Business.Contracts;

public interface ICartItemService
{
    Task<DataResult<IEnumerable<CartItem>>> GetByUserAsync(string userId);  
    Task<Result> AddAsync(CartItem cartItem);
    Task<Result> UpdateQuantityAsync(Guid cartItemId, int quantity);
    Task<Result> RemoveAsync(Guid cartItemId);
    Task<Result> ClearAsync(string userId);
}
