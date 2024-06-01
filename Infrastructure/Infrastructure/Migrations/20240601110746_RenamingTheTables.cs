using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamingTheTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_ProductFeedbacks_ProductFeedbackEntityProductId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Ratings_RatingId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_ReviewTexts_ReviewId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewTexts",
                table: "ReviewTexts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.RenameTable(
                name: "ReviewTexts",
                newName: "UserReviews");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "UserFeedbacks");

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "UserRatings");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_ReviewId",
                table: "UserFeedbacks",
                newName: "IX_UserFeedbacks_ReviewId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_RatingId",
                table: "UserFeedbacks",
                newName: "IX_UserFeedbacks_RatingId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_ProductFeedbackEntityProductId",
                table: "UserFeedbacks",
                newName: "IX_UserFeedbacks_ProductFeedbackEntityProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserReviews",
                table: "UserReviews",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFeedbacks",
                table: "UserFeedbacks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRatings",
                table: "UserRatings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFeedbacks_ProductFeedbacks_ProductFeedbackEntityProductId",
                table: "UserFeedbacks",
                column: "ProductFeedbackEntityProductId",
                principalTable: "ProductFeedbacks",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFeedbacks_UserRatings_RatingId",
                table: "UserFeedbacks",
                column: "RatingId",
                principalTable: "UserRatings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFeedbacks_UserReviews_ReviewId",
                table: "UserFeedbacks",
                column: "ReviewId",
                principalTable: "UserReviews",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFeedbacks_ProductFeedbacks_ProductFeedbackEntityProductId",
                table: "UserFeedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFeedbacks_UserRatings_RatingId",
                table: "UserFeedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFeedbacks_UserReviews_ReviewId",
                table: "UserFeedbacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserReviews",
                table: "UserReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRatings",
                table: "UserRatings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFeedbacks",
                table: "UserFeedbacks");

            migrationBuilder.RenameTable(
                name: "UserReviews",
                newName: "ReviewTexts");

            migrationBuilder.RenameTable(
                name: "UserRatings",
                newName: "Ratings");

            migrationBuilder.RenameTable(
                name: "UserFeedbacks",
                newName: "Reviews");

            migrationBuilder.RenameIndex(
                name: "IX_UserFeedbacks_ReviewId",
                table: "Reviews",
                newName: "IX_Reviews_ReviewId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFeedbacks_RatingId",
                table: "Reviews",
                newName: "IX_Reviews_RatingId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFeedbacks_ProductFeedbackEntityProductId",
                table: "Reviews",
                newName: "IX_Reviews_ProductFeedbackEntityProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewTexts",
                table: "ReviewTexts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ProductFeedbacks_ProductFeedbackEntityProductId",
                table: "Reviews",
                column: "ProductFeedbackEntityProductId",
                principalTable: "ProductFeedbacks",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Ratings_RatingId",
                table: "Reviews",
                column: "RatingId",
                principalTable: "Ratings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ReviewTexts_ReviewId",
                table: "Reviews",
                column: "ReviewId",
                principalTable: "ReviewTexts",
                principalColumn: "Id");
        }
    }
}
