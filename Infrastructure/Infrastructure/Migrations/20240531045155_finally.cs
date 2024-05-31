using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class @finally : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductReviews",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProductReviews");

            migrationBuilder.RenameTable(
                name: "ProductReviews",
                newName: "ReviewTexts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewTexts",
                table: "ReviewTexts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProductFeedbacks",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    RatingCount = table.Column<int>(type: "int", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFeedbacks", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductFeedbackEntityProductId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_ProductFeedbacks_ProductFeedbackEntityProductId",
                        column: x => x.ProductFeedbackEntityProductId,
                        principalTable: "ProductFeedbacks",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_Reviews_Ratings_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Ratings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_ReviewTexts_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "ReviewTexts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductFeedbackEntityProductId",
                table: "Reviews",
                column: "ProductFeedbackEntityProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RatingId",
                table: "Reviews",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewId",
                table: "Reviews",
                column: "ReviewId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "ProductFeedbacks");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewTexts",
                table: "ReviewTexts");

            migrationBuilder.RenameTable(
                name: "ReviewTexts",
                newName: "ProductReviews");

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "ProductReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "ProductReviews",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProductReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductReviews",
                table: "ProductReviews",
                column: "Id");
        }
    }
}
