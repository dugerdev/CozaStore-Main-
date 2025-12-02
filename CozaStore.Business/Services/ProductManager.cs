using CozaStore.Business.Contracts;      // IProductService sözleşmesi
using CozaStore.Core.DataAccess;         // IUnitOfWork erişimi
using CozaStore.Core.Utilities.Results;  // Result/DataResult tipleri
using CozaStore.Entities.Entities;       // Product entity’si
using FluentValidation;                  // FluentValidation API’si

namespace CozaStore.Business.Services;

/// <summary>
/// Ürünlere ilişkin iş kurallarını (validation, repository çağrıları, soft delete vb.) yöneten servis.
/// </summary>
public class ProductManager : IProductService
{
    private readonly IUnitOfWork _unitOfWork;           // Tüm repository’leri yöneten yapı
    private readonly IValidator<Product> _validator;    // Ürün doğrulama kuralları

    public ProductManager(IUnitOfWork unitOfWork, IValidator<Product> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<DataResult<Product>> GetByIdAsync(Guid id)
    {
        // Soft delete edilmemiş ürünü getir
        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product is null)
            return new ErrorDataResult<Product>(null!, "Ürün bulunamadı.");

        return new SuccessDataResult<Product>(product);
    }

    public async Task<DataResult<IEnumerable<Product>>> GetAllAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return new SuccessDataResult<IEnumerable<Product>>(products);
    }

    public async Task<DataResult<IEnumerable<Product>>> GetByCategoryAsync(Guid categoryId)
    {
        var products = await _unitOfWork.Products.FindAsync(p => p.CategoryId == categoryId);
        return new SuccessDataResult<IEnumerable<Product>>(products);
    }

    public async Task<Result> AddAsync(Product product)
    {
        await _validator.ValidateAndThrowAsync(product);      // FluentValidation kontrolü

        await _unitOfWork.Products.AddAsync(product);         // Repository üzerinden ekle
        await _unitOfWork.SaveChangesAsync();                 // Kalıcı hale getir

        return new SuccessResult("Ürün eklendi.");
    }

    public async Task<Result> UpdateAsync(Product product)
    {
        await _validator.ValidateAndThrowAsync(product);

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Ürün güncellendi.");
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        await _unitOfWork.Products.SoftDeleteAsync(id);       // Soft delete uygula
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Ürün silindi.");
    }
}
