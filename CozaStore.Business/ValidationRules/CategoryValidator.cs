using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.ValidationRules;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Kategori adı zorunludur.")
            .MaximumLength(100).WithMessage("Kategori adı 100 karakterden uzun olamaz.");

        RuleFor(c => c.Description)
            .MaximumLength(500).WithMessage("Kategori açıklaması 500 karakteri aşamaz.");

    }

  
}
