using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;

namespace CozaStore.Business.Contracts;

public interface IProductService
{
    Task<DataResult<Product>> GetByIdAsync(Guid id);
    Task<DataResult<IEnumerable<Product>>> GetAllAsync();
    Task<DataResult<IEnumerable<Product>>> GetByCategoryAsync(Guid categoryId);
    Task<Result> AddAsync(Product product);
    Task<Result> UpdateAsync(Product product);
    Task<Result> DeleteAsync(Guid id);
}
