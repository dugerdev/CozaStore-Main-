using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.ValidationRules;

public class ReviewValidator : AbstractValidator<Review>
{
    public ReviewValidator()
    {
        RuleFor(r => r.ProductId)
            .NotEmpty().WithMessage("Ürün bilgisi zorunludur.");

        RuleFor(r => r.UserId)
            .NotEmpty().WithMessage("Kullanıcı bilgisi zorunludur.")
            .MaximumLength(450);

        RuleFor(r => r.Title)
            .MaximumLength(200);

        RuleFor(r => r.Comment)
            .MaximumLength(2000);

        RuleFor(r => r.Rating)
            .InclusiveBetween(1, 5).WithMessage("Puan 1 ile 5 arasında olmalıdır.");
    }
}