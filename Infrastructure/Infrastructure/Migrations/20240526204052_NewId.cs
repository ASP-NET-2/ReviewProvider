using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductReviews",
                table: "ProductReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductRatings",
                table: "ProductRatings");

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "ProductReviews",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProductReviews",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "ProductRatings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProductRatings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductReviews",
                table: "ProductReviews",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductRatings",
                table: "ProductRatings",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductReviews",
                table: "ProductReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductRatings",
                table: "ProductRatings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductRatings");

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "ProductReviews",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "ProductRatings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductReviews",
                table: "ProductReviews",
                column: "ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductRatings",
                table: "ProductRatings",
                column: "ProductId");
        }
    }
}
