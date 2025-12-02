using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CozaStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseImageUrlLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                comment: "Ürün görseli URL'i",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "Ürün görseli URL'i");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Categories",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                comment: "Kategori görseli URL'i",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "Kategori görseli URL'i");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Ürün görseli URL'i",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true,
                oldComment: "Ürün görseli URL'i");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Categories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Kategori görseli URL'i",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true,
                oldComment: "Kategori görseli URL'i");
        }
    }
}
