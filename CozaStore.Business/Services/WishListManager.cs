using CozaStore.Business.Contracts;      // IWishListService sözleşmesi
using CozaStore.Core.DataAccess;         // IUnitOfWork erişimi
using CozaStore.Core.Utilities.Results;  // Result/DataResult tipleri
using CozaStore.Entities.Entities;       // WishList entity'si
using FluentValidation;                  // FluentValidation API'si

namespace CozaStore.Business.Services;

/// <summary>
/// İstek listesi (WishList) işlemlerini yöneten servis.
/// Kullanıcıların beğendikleri ürünleri ekleyip çıkarabildiği işlemleri içerir.
/// </summary>
public class WishListManager : IWishListService
{
    private readonly IUnitOfWork _unitOfWork;              // Tüm repository'leri yöneten yapı
    private readonly IValidator<WishList> _validator;       // WishList doğrulama kuralları

    public WishListManager(IUnitOfWork unitOfWork, IValidator<WishList> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    /// <summary>
    /// Belirli bir kullanıcının istek listesindeki tüm ürünleri getirir.
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si (Identity User ID - string)</param>
    /// <returns>Kullanıcının istek listesindeki ürünler</returns>
    public async Task<DataResult<IEnumerable<WishList>>> GetByUserAsync(string userId)
    {
        // Kullanıcı ID'sine göre istek listesindeki ürünleri bul
        var wishLists = await _unitOfWork.WishLists.FindAsync(w => w.UserId == userId);

        return new SuccessDataResult<IEnumerable<WishList>>(wishLists);
    }

    /// <summary>
    /// İstek listesine yeni bir ürün ekler.
    /// Aynı ürün zaten listede varsa hata döner.
    /// </summary>
    /// <param name="wishList">Eklenecek istek listesi kaydı</param>
    /// <returns>İşlem sonucu</returns>
    public async Task<Result> AddAsync(WishList wishList)
    {
        // FluentValidation ile doğrulama yap
        await _validator.ValidateAndThrowAsync(wishList);

        // Aynı kullanıcı için aynı ürün zaten listede mi kontrol et
        var existingWishList = await _unitOfWork.WishLists.FindAsync(
            w => w.UserId == wishList.UserId && w.ProductId == wishList.ProductId);

        if (existingWishList.Any())
        {
            return new ErrorResult("Bu ürün zaten istek listenizde bulunmaktadır.");
        }

        // İstek listesine ekle
        await _unitOfWork.WishLists.AddAsync(wishList);
        await _unitOfWork.SaveChangesAsync();              // Kalıcı hale getir

        return new SuccessResult("Ürün istek listenize eklendi.");
    }

    /// <summary>
    /// İstek listesinden bir ürünü çıkarır (soft delete).
    /// </summary>
    /// <param name="wishListId">Silinecek istek listesi kaydının ID'si</param>
    /// <returns>İşlem sonucu</returns>
    public async Task<Result> RemoveAsync(Guid wishListId)
    {
        // İstek listesi kaydını bul
        var wishList = await _unitOfWork.WishLists.GetByIdAsync(wishListId);

        if (wishList is null)
        {
            return new ErrorResult("İstek listesi kaydı bulunamadı.");
        }

        // Soft delete uygula
        await _unitOfWork.WishLists.SoftDeleteAsync(wishListId);
        await _unitOfWork.SaveChangesAsync();

        return new SuccessResult("Ürün istek listenizden çıkarıldı.");
    }
}


