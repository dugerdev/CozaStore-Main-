using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CozaStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false, comment: "Kullanıcı Id değeri"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Adres başlığı"),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Adres satırı 1"),
                    AddressLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "Adres satırı 2"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Şehir"),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "İlçe"),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Posta kodu"),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "Turkey", comment: "Ülke"),
                    AddressType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Adres tipi (Shipping / Billing)"),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Varsayılan adres mi?"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Adres oluşturulma tarihi (UTC)"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Adres güncellenme tarihi (UTC)"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Kategori adı"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Kategori açıklaması"),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Kategori görseli URL'i"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Kaydın oluşturulma tarihi (UTC)"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Kaydın son güncellenme tarihi (UTC)"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Kayıt aktif mi?"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Soft delete flag'i"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Kayıt silindiyse silinme tarihi (UTC)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Sipariş numarası"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Sipariş tarihi (UTC)"),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "Sipariş toplam tutarı"),
                    ShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m, comment: "Kargo ücreti"),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m, comment: "Vergi tutarı"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending", comment: "Sipariş durumu"),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Unpaid", comment: "Ödeme durumu"),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Ödeme yöntemi"),
                    ShippingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Teslimat adresi Id'si"),
                    BillingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "Fatura adresi Id'si"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false, comment: "Siparişi veren kullanıcının Id değeri"),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Sipariş ile ilgili notlar"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Sipariş oluşturulma tarihi (UTC)"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Sipariş güncellenme tarihi (UTC)"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Sipariş kaydı aktif mi?"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Sipariş soft delete flag'i"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Sipariş silindiyse silinme tarihi")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Ürün adı"),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true, comment: "Ürün açıklaması"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "Ürün satış fiyatı"),
                    DiscountPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true, comment: "Ürünün indirimli fiyatı"),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Ürün görseli URL'i"),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Stock Keeping Unit (stok kodu)"),
                    StockQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "Stok adedi"),
                    IsInStock = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Ürün stokta mı?"),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Kategori foreign key değeri"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Kaydın oluşturulma tarihi (UTC)"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Kaydın son güncellenme tarihi (UTC)"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Kayıt aktif mi?"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Soft delete flag'i"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Kayıt silindiyse silinme tarihi (UTC)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false, comment: "Kullanıcı Id değeri"),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Ürün Id değeri"),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "Sepete eklenen ürün adedi"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Sepet kaydı oluşturulma tarihi (UTC)"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Sepet kaydı güncellenme tarihi (UTC)"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Sepet kaydı aktif mi?"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Soft delete flag'i"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Sipariş foreign key değeri"),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Ürün foreign key değeri"),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Sipariş anındaki ürün adı"),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "Sipariş anındaki birim fiyat"),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Sipariş edilen miktar"),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "Satır toplamı (UnitPrice * Quantity)"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Sipariş detayı oluşturulma tarihi (UTC)"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Sipariş detayı güncellenme tarihi (UTC)"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Kayıt aktif mi?"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Soft delete flag'i"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Ürün foreign key değeri"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false, comment: "Yorumu yapan kullanıcı Id'si"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "Yorum başlığı"),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true, comment: "Yorum metni"),
                    Rating = table.Column<int>(type: "int", nullable: false, comment: "Puan (1-5)"),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Yorum admin tarafından onaylandı mı?"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Yorum oluşturulma tarihi (UTC)"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Yorum güncellenme tarihi (UTC)"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WishLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false, comment: "Kullanıcı Id değeri"),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Ürün Id değeri"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "İstek listesi kaydı oluşturulma tarihi (UTC)"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "İstek listesi kaydı güncellenme tarihi (UTC)"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Kayıt aktif mi?"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Soft delete flag'i"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishLists_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_IsActive",
                table: "Addresses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_IsDeleted",
                table: "Addresses",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_User_Default",
                table: "Addresses",
                columns: new[] { "UserId", "IsDefault" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_IsActive",
                table: "CartItems",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_IsDeleted",
                table: "CartItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_User_Product_Unique",
                table: "CartItems",
                columns: new[] { "UserId", "ProductId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsActive",
                table: "Categories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_IsActive",
                table: "OrderDetails",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_IsDeleted",
                table: "OrderDetails",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BillingAddressId",
                table: "Orders",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsActive",
                table: "Orders",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsDeleted",
                table: "Orders",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDate",
                table: "Orders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingAddressId",
                table: "Orders",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsDeleted",
                table: "Products",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU_NotNull",
                table: "Products",
                column: "SKU",
                unique: true,
                filter: "[SKU] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_IsActive",
                table: "Reviews",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_IsApproved",
                table: "Reviews",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_IsDeleted",
                table: "Reviews",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Product_Approval",
                table: "Reviews",
                columns: new[] { "ProductId", "IsApproved" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WishLists_IsActive",
                table: "WishLists",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WishLists_IsDeleted",
                table: "WishLists",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_WishLists_ProductId",
                table: "WishLists",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WishLists_User_Product_Unique",
                table: "WishLists",
                columns: new[] { "UserId", "ProductId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_WishLists_UserId",
                table: "WishLists",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "WishLists");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
