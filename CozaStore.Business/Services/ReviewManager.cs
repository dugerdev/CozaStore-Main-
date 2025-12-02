using CozaStore.Business.Contracts;
using CozaStore.Core.DataAccess;
using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;
using FluentValidation;

namespace CozaStore.Business.Services;

/// <summary>
/// Ürün yorumlarını yönetir (ekleme, onaylama, silme).
/// </summary>
public class ReviewManager : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<Review> _validator;

    public ReviewManager(IUnitOfWork unitOfWork, IValidator<Review> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<DataResult<IEnumerable<Review>>> GetAllAsync()
    {
        var reviews = await _unitOfWork.Reviews.GetAllAsync();
        return new SuccessDataResult<IEnumerable<Review>>(reviews);
    }

    public async Task<DataResult<IEnumerable<Review>>> GetByProductAsync(Guid productId)
    {
        var reviews = await _unitOfWork.Reviews.FindAsync(r => r.ProductId == productId && r.IsApproved);
        return new SuccessDataResult<IEnumerable<Review>>(reviews);
    }

    public async Task<DataResult<IEnumerable<Review>>> GetPendingAsync()
    {
        var reviews = await _unitOfWork.Reviews.FindAsync(r => !r.IsApproved);
        return new SuccessDataResult<IEnumerable<Review>>(reviews);
    }

    public async Task<Result> AddAsync(Review review)
    {
        await _validator.ValidateAndThrowAsync(review);

        review.IsApproved = false; // Moderasyon sürecinden geçecek
        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Yorum kaydedildi. Onay bekliyor.");
    }

    public async Task<Result> ApproveAsync(Guid reviewId)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
        if (review is null)
            return new ErrorResult("Yorum bulunamadı.");

        review.IsApproved = true;
        await _unitOfWork.Reviews.UpdateAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Yorum onaylandı.");
    }

    public async Task<Result> RejectAsync(Guid reviewId)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
        if (review is null)
            return new ErrorResult("Yorum bulunamadı.");

        // Reject işlemi için yorumu soft delete yapıyoruz
        await _unitOfWork.Reviews.SoftDeleteAsync(reviewId);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Yorum reddedildi.");
    }

    public async Task<Result> DeleteAsync(Guid reviewId)
    {
        await _unitOfWork.Reviews.SoftDeleteAsync(reviewId);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Yorum silindi.");
    }
}





