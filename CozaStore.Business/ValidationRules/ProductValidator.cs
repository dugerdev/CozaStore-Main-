using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.ValidationRules;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Ürün Adı Boş Olamaz.")
            .MaximumLength(200);

        RuleFor(p => p.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Fiyat Negatıf Olamaz");

        RuleFor(p => p.CategoryId)
            .NotEmpty().WithMessage("Kategori Seçilmelidir.");
    }
}
