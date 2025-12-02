using CozaStore.Business.Contracts;
using CozaStore.Core.DataAccess;
using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;
using CozaStore.Entities.Enums;
using FluentValidation;

namespace CozaStore.Business.Services;

/// <summary>
/// Siparişlerin oluşturulması, durumu ve silinmesiyle ilgili iş kurallarını uygular.
/// </summary>
public class OrderManager : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<Order> _orderValidator;
    private readonly IValidator<OrderDetail> _detailValidator;

    public OrderManager(IUnitOfWork unitOfWork,
                        IValidator<Order> orderValidator,
                        IValidator<OrderDetail> detailValidator)
    {
        _unitOfWork = unitOfWork;
        _orderValidator = orderValidator;
        _detailValidator = detailValidator;
    }

    public async Task<DataResult<Order>> GetByIdAsync(Guid id)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order is null)
            return new ErrorDataResult<Order>(null!, "Sipariş bulunamadı.");

        return new SuccessDataResult<Order>(order);
    }

    public async Task<DataResult<IEnumerable<Order>>> GetAllAsync()
    {
        var orders = await _unitOfWork.Orders.GetAllAsync();
        return new SuccessDataResult<IEnumerable<Order>>(orders);
    }

    public async Task<DataResult<IEnumerable<Order>>> GetByUserAsync(string userId)
    {
        var orders = await _unitOfWork.Orders.FindAsync(o => o.UserId == userId);
        return new SuccessDataResult<IEnumerable<Order>>(orders);
    }

    public async Task<Result> AddAsync(Order order, IEnumerable<OrderDetail> orderDetails)
    {
        await _orderValidator.ValidateAndThrowAsync(order);

        // Order önce kaydedilmeli ki OrderId oluşsun
        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();          // Order Id üretimi için önce siparişi kaydet

        // OrderId atandıktan sonra OrderDetail'leri validate et
        foreach (var detail in orderDetails)
        {
            detail.OrderId = order.Id;                 // Sipariş detaylarına ilişkiyi setle
            await _detailValidator.ValidateAndThrowAsync(detail); // OrderId atandıktan sonra validate et
        }

        await _unitOfWork.OrderDetails.AddRangeAsync(orderDetails);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Sipariş oluşturuldu.");
    }

    public async Task<Result> UpdateStatusAsync(Guid id, OrderStatus status)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order is null)
            return new ErrorResult("Sipariş bulunamadı.");

        order.Status = status;
        await _unitOfWork.Orders.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Sipariş durumu güncellendi.");
    }

    public async Task<Result> UpdatePaymentStatusAsync(Guid id, PaymentStatus paymentStatus)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order is null)
            return new ErrorResult("Sipariş bulunamadı.");

        order.PaymentStatus = paymentStatus;
        await _unitOfWork.Orders.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Ödeme durumu güncellendi.");
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        await _unitOfWork.Orders.SoftDeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Sipariş silindi.");
    }
}
