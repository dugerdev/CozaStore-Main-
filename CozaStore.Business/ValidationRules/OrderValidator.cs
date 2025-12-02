using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.ValidationRules
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(o => o.OrderNumber)
                .NotEmpty().WithMessage("Sipariş numarası zorunludur.")
                .MaximumLength(50);

            RuleFor(o => o.UserId)
                .NotEmpty().WithMessage("Kullanıcı bilgisi zorunludur.")
                .MaximumLength(450);

            RuleFor(o => o.TotalAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Toplam tutar negatif olamaz.");

            RuleFor(o => o.ShippingCost)
                .GreaterThanOrEqualTo(0).WithMessage("Kargo ücreti negatif olamaz.");

            RuleFor(o => o.TaxAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Vergi tutarı negaif olamaz.");
        }
    }
}
