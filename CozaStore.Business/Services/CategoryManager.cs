using CozaStore.Business.Contracts;      // ICategoryService sözleşmesi
using CozaStore.Core.DataAccess;         // IUnitOfWork erişimi
using CozaStore.Core.Utilities.Results;  // Result/DataResult tipleri
using CozaStore.Entities.Entities;       // Category entity’si
using FluentValidation;                  // FluentValidation API’si

namespace CozaStore.Business.Services;

/// <summary>
/// Kategorilere ilişkin iş kurallarını yöneten servis.
/// </summary>
public class CategoryManager : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<Category> _validator;

    public CategoryManager(IUnitOfWork unitOfWork, IValidator<Category> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<DataResult<Category>> GetByIdAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category is null)
            return new ErrorDataResult<Category>(null!, "Kategori bulunamadı.");

        return new SuccessDataResult<Category>(category);
    }

    public async Task<DataResult<IEnumerable<Category>>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return new SuccessDataResult<IEnumerable<Category>>(categories);
    }

    public async Task<Result> AddAsync(Category category)
    {
        await _validator.ValidateAndThrowAsync(category);

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Kategori eklendi.");
    }

    public async Task<Result> UpdateAsync(Category category)
    {
        await _validator.ValidateAndThrowAsync(category);

        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Kategori güncellendi.");
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        await _unitOfWork.Categories.SoftDeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return new SuccessResult("Kategori silindi.");
    }
}


