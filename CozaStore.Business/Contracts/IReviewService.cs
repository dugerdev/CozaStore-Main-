using CozaStore.Core.Utilities.Results;
using CozaStore.Entities.Entities;

namespace CozaStore.Business.Contracts;

public interface IReviewService
{
    Task<DataResult<IEnumerable<Review>>> GetAllAsync();
    Task<DataResult<IEnumerable<Review>>> GetByProductAsync(Guid productId);
    Task<DataResult<IEnumerable<Review>>> GetPendingAsync(); // Onay bekleyen yorumlar
    Task<Result> AddAsync(Review review);
    Task<Result> ApproveAsync(Guid reviewId);
    Task<Result> RejectAsync(Guid reviewId);
    Task<Result> DeleteAsync(Guid reviewId);
}