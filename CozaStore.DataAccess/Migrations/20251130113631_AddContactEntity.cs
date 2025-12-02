using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CozaStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddContactEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Gönderenin email adresi"),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false, comment: "Gönderilen mesaj içeriği"),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Mesaj okundu mu?"),
                    ReadDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Mesaj okunma tarihi"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "İletişim mesajı oluşturulma tarihi (UTC)"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "İletişim mesajı güncellenme tarihi (UTC)"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CreatedDate",
                table: "Contacts",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Email",
                table: "Contacts",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_IsActive",
                table: "Contacts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_IsDeleted",
                table: "Contacts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_IsRead",
                table: "Contacts",
                column: "IsRead");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");
        }
    }
}
