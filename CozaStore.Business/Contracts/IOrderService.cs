using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;
using CozaStore.Entities.Enums;

namespace CozaStore.Business.Contracts;

public interface IOrderService
{
    Task<DataResult<Order>> GetByIdAsync(Guid id);
    Task<DataResult<IEnumerable<Order>>> GetAllAsync();
    Task<DataResult<IEnumerable<Order>>> GetByUserAsync(string userId);
    Task<Result> AddAsync(Order order, IEnumerable<OrderDetail> orderDetails);
    Task<Result> UpdateStatusAsync(Guid id, OrderStatus status);
    Task<Result> UpdatePaymentStatusAsync(Guid id, PaymentStatus paymentStatus);
    Task<Result> DeleteAsync(Guid id);
}



