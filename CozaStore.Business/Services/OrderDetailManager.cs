using CozaStore.Business.Contracts;
using CozaStore.Core.DataAccess;
using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.Services;

/// <summary>
/// Sipariş detay kayıtlarını yöneten servis.
/// </summary>
public class OrderDetailManager : IOrderDetailService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<OrderDetail> _validator;

    public OrderDetailManager(IUnitOfWork unitOfWork, IValidator<OrderDetail> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<DataResult<IEnumerable<OrderDetail>>> GetByOrderIdAsync(Guid orderId)
    {
        var details = await _unitOfWork.OrderDetails.FindAsync(od => od.OrderId == orderId);
        return new SuccessDataResult<IEnumerable<OrderDetail>>(details);
    }

    public async Task<Result> AddRangeAsync(IEnumerable<OrderDetail> orderDetails)
    {
        foreach (var detail in orderDetails)
        {
            await _validator.ValidateAndThrowAsync(detail);
        }

        await _unitOfWork.OrderDetails.AddRangeAsync(orderDetails);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Sipariş detayları eklendi.");
    }

    public async Task<Result> DeleteByOrderIdAsync(Guid orderId)
    {
        var details = await _unitOfWork.OrderDetails.FindAsync(od => od.OrderId == orderId);
        foreach (var detail in details)
        {
            await _unitOfWork.OrderDetails.DeleteAsync(detail);
        }

        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Sipariş detayları silindi.");
    }
}





