using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;

namespace CozaStore.Business.Contracts;

public interface ICategoryService
{
    Task<DataResult<Category>> GetByIdAsync(Guid id);
    Task<DataResult<IEnumerable<Category>>> GetAllAsync();
    Task<Result> AddAsync(Category category);
    Task<Result> UpdateAsync(Category category);
    Task<Result> DeleteAsync(Guid id);
}
