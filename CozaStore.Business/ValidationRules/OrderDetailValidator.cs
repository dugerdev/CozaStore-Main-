using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.ValidationRules;

public class OrderDetailValidator : AbstractValidator<OrderDetail>
{
    public OrderDetailValidator()
    {
        RuleFor(od => od.OrderId)
            .NotEmpty().WithMessage("Sipariş bilgisi zorunludur.");

        RuleFor(od => od.ProductId)
            .NotEmpty().WithMessage("Ürün bilgisi zorunludur.");

        RuleFor(od => od.ProductName)
            .NotEmpty().WithMessage("Ürün bilgisi zorunludur")
            .MaximumLength(200);

        RuleFor(od => od.Quantity)
            .GreaterThan(0).WithMessage("Miktar en az 1 olmalıdır.");

        RuleFor(od => od.SubTotal)
            .GreaterThanOrEqualTo(0).WithMessage("Ara toplam negatif olamaz");
    }
}
