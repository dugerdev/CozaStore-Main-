using CozaStore.Business.Contracts;
using CozaStore.Business.Services;
using CozaStore.Business.ValidationRules;
using CozaStore.Entities.Entities;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CozaStore.Business.DependencyInjection;

/// <summary>
/// Business katmanının servis ve validator bağımlılıklarını kaydeden extension.
/// DbContext ve UnitOfWork kayıtları uygulama girişinde (Program.cs) yapılır.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddCozaStoreBusiness(this IServiceCollection services)
    {
        // Manager (service) kayıtları
        services.AddScoped<IProductService, ProductManager>();
        services.AddScoped<ICategoryService, CategoryManager>();
        services.AddScoped<IOrderService, OrderManager>();
        services.AddScoped<IOrderDetailService, OrderDetailManager>();
        services.AddScoped<ICartItemService, CartItemManager>();
        services.AddScoped<IAddressService, AddressManager>();
        services.AddScoped<IReviewService, ReviewManager>();
        services.AddScoped<IWishListService, WishListManager>();
        services.AddScoped<IContactService, ContactManager>();
        services.AddScoped<IBlogPostService, BlogPostManager>();

        // FluentValidation kayıtları
        services.AddScoped<IValidator<Product>, ProductValidator>();
        services.AddScoped<IValidator<Category>, CategoryValidator>();
        services.AddScoped<IValidator<Order>, OrderValidator>();
        services.AddScoped<IValidator<OrderDetail>, OrderDetailValidator>();
        services.AddScoped<IValidator<CartItem>, CartItemValidator>();
        services.AddScoped<IValidator<Address>, AddressValidator>();
        services.AddScoped<IValidator<Review>, ReviewValidator>();
        services.AddScoped<IValidator<WishList>, WishListValidator>();
        services.AddScoped<IValidator<Contact>, ContactValidator>();
        services.AddScoped<IValidator<BlogPost>, BlogPostValidator>();

        return services;
    }
}
