using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductRatings",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRatings", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "ProductReviews",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReviewTitle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviews", x => x.ProductId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductRatings");

            migrationBuilder.DropTable(
                name: "ProductReviews");
        }
    }
}
