using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;

namespace CozaStore.Business.Contracts;

public interface IAddressService
{
    Task<DataResult<Address>> GetByIdAsync(Guid id);
    Task<DataResult<IEnumerable<Address>>> GetByUserAsync(string userId);
    Task<Result> AddAsync(Address address);
    Task<Result> UpdateAsync(Address address);
    Task<Result> DeleteAsync(Guid id);
    Task<Result> SetDefaultAsync(string userId, Guid addressId);
}