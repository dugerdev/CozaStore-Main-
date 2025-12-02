using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.ValidationRules;

internal class ContactValidator : AbstractValidator<Contact>
{
    public ContactValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email adresi zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
            .MaximumLength(255).WithMessage("Email adresi en fazla 255 karakter olabilir.");

        RuleFor(c => c.Message)
            .NotEmpty().WithMessage("Mesaj içeriği zorunludur.")
            .MinimumLength(10).WithMessage("Mesaj en az 10 karakter olmalıdır.")
            .MaximumLength(2000).WithMessage("Mesaj en fazla 2000 karakter olabilir.");
    }
}


