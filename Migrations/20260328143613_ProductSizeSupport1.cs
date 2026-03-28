using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportShop.Migrations
{
    /// <inheritdoc />
    public partial class ProductSizeSupport1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_UserId_ProductId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "Products",
                newName: "SizeType");

            migrationBuilder.AddColumn<string>(
                name: "AvailableSizes",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SelectedSize",
                table: "CartItems",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId_ProductId_SelectedSize",
                table: "CartItems",
                columns: new[] { "UserId", "ProductId", "SelectedSize" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_UserId_ProductId_SelectedSize",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "AvailableSizes",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SelectedSize",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "SizeType",
                table: "Products",
                newName: "Size");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId_ProductId",
                table: "CartItems",
                columns: new[] { "UserId", "ProductId" },
                unique: true);
        }
    }
}
