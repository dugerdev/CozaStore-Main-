using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.ValidationRules;

internal class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(a => a.UserId)
            .NotEmpty().WithMessage("Kullanıcı bilgisi zorunludur.")
            .MaximumLength(450);

        RuleFor(a => a.Title)
            .NotEmpty().WithMessage("Adres basligi zorunludur.")
            .MaximumLength(100);

        RuleFor(a => a.AddressLine1)
            .NotEmpty().WithMessage("Adres satırı 1 zorunludur.")
            .MaximumLength(200);

        RuleFor(a => a.AddressLine2)
            .MaximumLength(200);

        RuleFor(a => a.City)
            .NotEmpty().WithMessage("Şehir bilgisi zorunludur.")
            .MaximumLength(100);

        RuleFor(a => a.District)
            .NotEmpty().WithMessage("İlçe bilgisi zorunludur.")
            .MaximumLength(100);

        RuleFor(a => a.PostalCode)
            .MaximumLength(20);

        RuleFor(a => a.Country)
            .NotEmpty().WithMessage("Ulke bilgisi zorunludur.")
            .MaximumLength(100);
    }
}
