using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;

namespace CozaStore.Business.Contracts;

public interface IWishListService
{
    Task<DataResult<IEnumerable<WishList>>> GetByUserAsync(string userId);
    Task<Result> AddAsync(WishList wishList);
    Task<Result> RemoveAsync(Guid wishListId);
}





