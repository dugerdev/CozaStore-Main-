using CozaStore.Business.Contracts;
using CozaStore.Core.DataAccess;
using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.Services;

/// <summary>
/// Blog gönderilerine ilişkin iş kurallarını yöneten servis.
/// </summary>
public class BlogPostManager : IBlogPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<BlogPost> _validator;

    public BlogPostManager(IUnitOfWork unitOfWork, IValidator<BlogPost> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<DataResult<BlogPost>> GetByIdAsync(Guid id)
    {
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(id);
        if (blogPost is null)
            return new ErrorDataResult<BlogPost>(null!, "Blog gönderisi bulunamadı.");

        return new SuccessDataResult<BlogPost>(blogPost);
    }

    public async Task<DataResult<IEnumerable<BlogPost>>> GetAllAsync()
    {
        var blogPosts = await _unitOfWork.BlogPosts.GetAllAsync();
        return new SuccessDataResult<IEnumerable<BlogPost>>(blogPosts);
    }

    public async Task<DataResult<IEnumerable<BlogPost>>> GetPublishedAsync()
    {
        var blogPosts = await _unitOfWork.BlogPosts.FindAsync(b => b.IsPublished && b.IsActive);
        return new SuccessDataResult<IEnumerable<BlogPost>>(blogPosts);
    }

    public async Task<Result> AddAsync(BlogPost blogPost)
    {
        await _validator.ValidateAndThrowAsync(blogPost);

        await _unitOfWork.BlogPosts.AddAsync(blogPost);
        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Blog gönderisi eklendi.");
    }

    public async Task<Result> UpdateAsync(BlogPost blogPost)
    {
        await _validator.ValidateAndThrowAsync(blogPost);

        await _unitOfWork.BlogPosts.UpdateAsync(blogPost);
        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Blog gönderisi güncellendi.");
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        await _unitOfWork.BlogPosts.SoftDeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Blog gönderisi silindi.");
    }
}

