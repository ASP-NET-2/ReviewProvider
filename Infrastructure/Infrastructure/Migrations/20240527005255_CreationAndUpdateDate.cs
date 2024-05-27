using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreationAndUpdateDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "ProductReviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OriginallyPostedDate",
                table: "ProductReviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "ProductRatings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OriginallyPostedDate",
                table: "ProductRatings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "OriginallyPostedDate",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "ProductRatings");

            migrationBuilder.DropColumn(
                name: "OriginallyPostedDate",
                table: "ProductRatings");
        }
    }
}
