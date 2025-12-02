using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;

namespace CozaStore.Business.Contracts;

public interface IOrderDetailService
{
    Task<DataResult<IEnumerable<OrderDetail>>> GetByOrderIdAsync(Guid orderId);
    Task<Result> AddRangeAsync(IEnumerable<OrderDetail> orderDetails);
    Task<Result> DeleteByOrderIdAsync(Guid orderId);
}
