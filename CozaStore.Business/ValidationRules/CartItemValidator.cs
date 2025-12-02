using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.ValidationRules
{
    public class CartItemValidator : AbstractValidator<CartItem>
    {
        public CartItemValidator()
        {
            RuleFor(ci => ci.UserId)
                .NotEmpty().WithMessage("Kullanıcı bilgisi zorunludur.")
                .MaximumLength(450);

            RuleFor(ci => ci.ProductId)
                .NotEmpty().WithMessage("Ürün bilgisi zorunludur.");

            RuleFor(ci => ci.Quantity)
                .GreaterThan(0).WithMessage("Sepete eklenen ürün miktarı en az 1 olmalıdır.");
        }
    }
}
