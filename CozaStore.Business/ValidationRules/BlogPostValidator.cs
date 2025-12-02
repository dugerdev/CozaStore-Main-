using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.ValidationRules;

public class BlogPostValidator : AbstractValidator<BlogPost>
{
    public BlogPostValidator()
    {
        RuleFor(b => b.Title)
            .NotEmpty().WithMessage("Blog gönderisi başlığı zorunludur.")
            .MaximumLength(200).WithMessage("Blog gönderisi başlığı 200 karakterden uzun olamaz.");

        RuleFor(b => b.Content)
            .NotEmpty().WithMessage("Blog gönderisi içeriği zorunludur.");

        RuleFor(b => b.AuthorId)
            .NotEqual(Guid.Empty).WithMessage("Yazar bilgisi zorunludur.");

        RuleFor(b => b.ImageUrl)
            .MaximumLength(2000).WithMessage("Görsel URL'i 2000 karakterden uzun olamaz.")
            .When(b => !string.IsNullOrEmpty(b.ImageUrl));
    }
}
