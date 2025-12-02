using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.ValidationRules;

public class WishListValidator : AbstractValidator<WishList>
{
    public WishListValidator()
    {
        RuleFor(w => w.UserId)
            .NotEmpty().WithMessage("Kullanıcı bilgisi zorunludur.")
            .MaximumLength(450);

        RuleFor(w => w.ProductId)
            .NotEmpty().WithMessage("Ürün bilgisi zorunludur.");
    }
}