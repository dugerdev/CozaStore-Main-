using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CozaStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBlogPostAuthorIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Blog gönderisi başlığı"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Blog gönderisi içeriği"),
                    ImageUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true, comment: "Blog gönderisi görseli URL'i"),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Blog gönderisi yayınlandı mı?"),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Blog gönderisini oluşturan kullanıcı ID'si"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Kaydın oluşturulma tarihi (UTC)"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Kaydın son güncellenme tarihi (UTC)"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Kayıt aktif mi?"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Soft delete flag'i"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Kayıt silindiyse silinme tarihi (UTC)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPosts_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_AuthorId",
                table: "BlogPosts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_IsActive",
                table: "BlogPosts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_IsDeleted",
                table: "BlogPosts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_IsPublished",
                table: "BlogPosts",
                column: "IsPublished");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPosts");
        }
    }
}
