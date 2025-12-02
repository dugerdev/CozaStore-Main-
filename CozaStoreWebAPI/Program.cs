using CozaStore.Business.DependencyInjection;
using CozaStore.Core.DataAccess;
using CozaStore.DataAccess.Data;
using CozaStore.DataAccess.Repositories;
using CozaStore.DataAccess.Seed;
using CozaStore.Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Model validation'ı devre dışı bırak (FluentValidation kullanıyoruz)
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(options =>
    {
        // JSON property isimlerini PascalCase olarak tut (camelCase'e çevirme)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection bulunamadı.");

// DbContext yapılandırması
builder.Services.AddDbContext<CozaStoreDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity yapılandırması
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Parola gereksinimleri
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    // Kullanıcı ayarları
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<CozaStoreDbContext>()
.AddDefaultTokenProviders();

// Business Layer servisleri
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddCozaStoreBusiness();

// JWT Authentication yapılandırması
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] 
    ?? throw new InvalidOperationException("JWT SecretKey bulunamadı.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.FromMinutes(5) // Token süresi için 5 dakika tolerans
    };
    
    // JWT Bearer events - debugging için
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var authHeader = context.Request.Headers["Authorization"].ToString();
            var exception = context.Exception;
            logger.LogError("JWT Authentication failed: {Error}, AuthHeader: {AuthHeader}, ExceptionType: {ExceptionType}, InnerException: {InnerException}", 
                exception?.Message ?? "No exception message", 
                authHeader?.Substring(0, Math.Min(100, authHeader?.Length ?? 0)) + "...",
                exception?.GetType().Name ?? "Unknown",
                exception?.InnerException?.Message ?? "No inner exception");
            
            // Exception detaylarını logla
            if (exception != null)
            {
                logger.LogError("Exception details: {Exception}", exception.ToString());
            }
            
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var authHeader = context.Request.Headers["Authorization"].ToString();
            logger.LogWarning("JWT Challenge: Error={Error}, ErrorDescription={ErrorDescription}, AuthHeader={AuthHeader}, RequestPath={RequestPath}", 
                context.Error ?? "null", 
                context.ErrorDescription ?? "null", 
                authHeader?.Substring(0, Math.Min(100, authHeader?.Length ?? 0)) + "...",
                context.Request.Path);
            
            // Challenge nedenini kontrol et
            if (string.IsNullOrEmpty(context.Error))
            {
                logger.LogWarning("Challenge triggered but no error message. This usually means token validation failed silently.");
            }
            
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("JWT Token validated successfully for user: {UserId}", 
                context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var authHeader = context.Request.Headers["Authorization"].ToString();
            var hasToken = !string.IsNullOrEmpty(authHeader);
            
            logger.LogError("JWT Message received. HasToken: {HasToken}, AuthHeader: {AuthHeader}, RequestPath: {RequestPath}", 
                hasToken, 
                authHeader != null ? authHeader.Substring(0, Math.Min(100, authHeader.Length)) + "..." : "null",
                context.Request.Path);
            
            // Token'ı parse etmeyi dene
            if (hasToken && authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                logger.LogError("Token extracted. Token length: {TokenLength}, Token preview: {TokenPreview}", 
                    token.Length, token.Substring(0, Math.Min(50, token.Length)) + "...");
            }
            else if (hasToken)
            {
                logger.LogError("Auth header exists but doesn't start with 'Bearer '. Header: {Header}", authHeader);
            }
            else
            {
                logger.LogError("No Authorization header found in request");
            }
            
            return Task.CompletedTask;
        }
    };
});

// CORS yapılandırması (WebUI'den istekler için)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebUI", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7002", 
                "http://localhost:5002",
                "http://localhost:5251"
              )
              .AllowAnyHeader() // Authorization header'ı dahil
              .AllowAnyMethod()
              .AllowCredentials()
              .WithExposedHeaders("Authorization"); // Authorization header'ını expose et
    });
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CozaStoreDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        
        await DbSeeder.SeedAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Development ayarları
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// CORS middleware'i - Authentication'dan önce olmalı
app.UseCors("AllowWebUI");

// Authentication ve Authorization middleware'leri
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();