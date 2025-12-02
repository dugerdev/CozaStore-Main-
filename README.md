# CozaStore - E-Commerce Platform

Modern ve ölçeklenebilir bir e-ticaret platformu. ASP.NET Core Web API ve MVC kullanılarak geliştirilmiştir.

## 🚀 Özellikler

### Kullanıcı Özellikleri
- ✅ Ürün listeleme ve detay sayfaları
- ✅ Kategori bazlı filtreleme
- ✅ Sepet yönetimi
- ✅ İstek listesi (Wishlist)
- ✅ Sipariş oluşturma ve takibi
- ✅ Ürün yorumlama ve değerlendirme
- ✅ İletişim formu
- ✅ Blog okuma

### Admin Paneli Özellikleri
- ✅ Ürün yönetimi (CRUD)
- ✅ Kategori yönetimi (CRUD)
- ✅ Sipariş yönetimi ve durum güncelleme
- ✅ Blog yönetimi (CRUD)
- ✅ İletişim mesajları yönetimi
- ✅ Ürün yorumları onaylama/reddetme
- ✅ Dashboard ve istatistikler

## 🏗️ Mimari

Proje **Clean Architecture** ve **SOLID** prensipleri kullanılarak geliştirilmiştir.

### Katmanlar

```
CozaStore/
├── CozaStoreWebAPI/          # REST API (Backend)
├── CozaStoreWebUI/           # MVC Web Application (Frontend)
├── CozaStore.Business/       # Business Logic & Validation
├── CozaStore.DataAccess/     # Data Access & Repository Pattern
├── CozaStore.Entities/       # Domain Entities
└── CozaStore.Core/           # Shared DTOs & Interfaces
```

### Teknolojiler

#### Backend (WebAPI)
- **Framework:** ASP.NET Core 9.0
- **ORM:** Entity Framework Core
- **Database:** SQL Server / SQLite
- **Authentication:** JWT Bearer Token
- **Validation:** FluentValidation
- **Architecture:** Repository Pattern, Unit of Work

#### Frontend (WebUI)
- **Framework:** ASP.NET Core MVC 9.0
- **Template Engine:** Razor Pages
- **UI Framework:** Bootstrap 5, AdminLTE 4
- **JavaScript:** jQuery, Vanilla JS
- **Icons:** Bootstrap Icons

## 📋 Gereksinimler

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server) veya SQLite
- [Visual Studio 2022](https://visualstudio.microsoft.com/) veya [VS Code](https://code.visualstudio.com/)

## 🛠️ Kurulum

### 1. Projeyi Klonlayın

```bash
git clone https://github.com/yourusername/CozaStore.git
cd CozaStore
```

### 2. Database Bağlantısını Yapılandırın

**WebAPI için:**
`CozaStoreWebAPI/appsettings.json` dosyasını düzenleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CozaStoreDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**WebUI için:**
`CozaStoreWebUI/CozaStore.WebUI/appsettings.json` dosyasını düzenleyin:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7001/api"
  }
}
```

### 3. Database Migration

```bash
cd CozaStoreWebAPI
dotnet ef database update
```

### 4. Projeleri Çalıştırın

**Terminal 1 - WebAPI:**
```bash
cd CozaStoreWebAPI
dotnet run
```

**Terminal 2 - WebUI:**
```bash
cd CozaStoreWebUI/CozaStore.WebUI
dotnet run
```

### 5. Tarayıcıda Açın

- **WebUI:** https://localhost:7002
- **WebAPI (Swagger):** https://localhost:7001/swagger

## 👤 Varsayılan Kullanıcılar

### Admin Hesabı
- **Email:** admin@cozastore.com
- **Password:** Admin123!

### Test Kullanıcısı
- **Email:** user@cozastore.com
- **Password:** User123!

## 📁 Proje Yapısı

### CozaStoreWebAPI (Backend)
```
Controllers/          # API Endpoints
├── AuthController.cs
├── ProductsController.cs
├── CategoriesController.cs
├── OrdersController.cs
└── ...
```

### CozaStoreWebUI (Frontend)
```
Controllers/          # MVC Controllers
Areas/
└── Admin/           # Admin Panel
    ├── Controllers/
    └── Views/
Views/               # Public Views
wwwroot/             # Static Files (CSS, JS, Images)
```

### CozaStore.Business
```
Services/            # Business Logic
ValidationRules/     # FluentValidation Rules
Contracts/           # Service Interfaces
```

### CozaStore.DataAccess
```
Data/                # DbContext
Repositories/        # Repository Pattern
Configuration/       # Entity Configurations
Migrations/          # EF Core Migrations
```

## 🔐 Güvenlik

- ✅ JWT Token Authentication
- ✅ Role-based Authorization (Admin, User)
- ✅ Anti-Forgery Token (CSRF Protection)
- ✅ Input Validation (FluentValidation)
- ✅ SQL Injection Protection (EF Core)
- ✅ XSS Protection

## 🧪 Test

```bash
# Unit testleri çalıştır
dotnet test

# Coverage raporu oluştur
dotnet test /p:CollectCoverage=true
```

## 📝 API Dokümantasyonu

API dokümantasyonu Swagger UI üzerinden erişilebilir:
- https://localhost:7001/swagger

### Örnek API Endpoints

```
GET    /api/products              # Tüm ürünleri listele
GET    /api/products/{id}         # Ürün detayı
POST   /api/products              # Yeni ürün ekle (Admin)
PUT    /api/products/{id}         # Ürün güncelle (Admin)
DELETE /api/products/{id}         # Ürün sil (Admin)

GET    /api/categories            # Kategorileri listele
POST   /api/auth/login            # Giriş yap
POST   /api/auth/register         # Kayıt ol
```

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'feat: Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request açın

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 📧 İletişim

Proje Sahibi - [@dugerdev](https://github.com/yourusername)

Proje Linki: [https://github.com/dugerdev/CozaStore](https://github.com/yourusername/CozaStore)

## 🙏 Teşekkürler

- [AdminLTE](https://adminlte.io/) - Admin panel template
- [CozaStore Template](https://colorlib.com/wp/template/cozastore/) - Frontend template
- ASP.NET Core Team

---

⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!



