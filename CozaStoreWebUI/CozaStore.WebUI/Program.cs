using CozaStore.Application.Services;
using CozaStore.WebUI.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Stripe;
using ReviewService = Stripe.ReviewService;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7087/api";

// IHttpContextAccessor'ı ekle (JWT token handler için gerekli)
builder.Services.AddHttpContextAccessor();

// Development'ta SSL sertifika doğrulamasını bypass et
HttpMessageHandler CreateHandler()
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
}

// JWT token handler'ı ekle
builder.Services.AddTransient<JwtTokenHandler>();

// HttpClient'ları yapılandır (JWT token handler ile)
builder.Services.AddHttpClient<CozaStore.Application.Services.ProductService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/products/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<CategoryService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/categories/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<CartService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/cartitems/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>()
.SetHandlerLifetime(TimeSpan.FromMinutes(5)); // Handler lifetime'ı ayarla

// AuthService için token handler ekleme (login/register için token yok)
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/auth/");
}).ConfigurePrimaryHttpMessageHandler(CreateHandler);

builder.Services.AddHttpClient<OrderService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/orders/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<AddressService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/addresses/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<ReviewService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/reviews/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<WishListService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/wishlist/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

// ContactService için token handler ekleme (herkes mesaj gönderebilir)
builder.Services.AddHttpClient<ContactService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/contacts/");
}).ConfigurePrimaryHttpMessageHandler(CreateHandler);

builder.Services.AddHttpClient<BlogService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/blogposts/");
}).ConfigurePrimaryHttpMessageHandler(CreateHandler);

// PaymentService'i kaydet
builder.Services.AddScoped<PaymentService>();

// Admin Servisleri - HttpClient yapılandırması
builder.Services.AddHttpClient<AdminProductService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/products/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<AdminCategoryService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/categories/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<AdminOrderService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/orders/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<AdminReviewService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/reviews/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<AdminContactService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/contacts/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<AdminBlogPostService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/blogposts/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<AdminDashboardService>(client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/dashboard/");
})
.ConfigurePrimaryHttpMessageHandler(CreateHandler)
.AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Session desteği ekle
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7); // 7 gün
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Name = ".CozaStore.Session";
});

// Cookie Authentication yapılandırması
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24); // 24 saat - JWT token ile uyumlu
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax; // Lax yapıldı - Admin area için gerekli
        options.Cookie.Path = "/"; // Tüm path'ler için geçerli
    });

//Stripe Configurasyonu
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Development'ta detaylı hata sayfası göster
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

// Authentication ve Authorization middleware'leri
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Area routing - Admin paneli için
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

// Default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
