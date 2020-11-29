using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Games4TradeAPI.Data.Migrations
{
    public partial class addedconsolesandgames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConsoleRegionId",
                table: "AdvertisementItems",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeveloped",
                table: "AdvertisementItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Developer",
                table: "AdvertisementItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameRegionId",
                table: "AdvertisementItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "AdvertisementItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AdvertisementItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementItems_ConsoleRegionId",
                table: "AdvertisementItems",
                column: "ConsoleRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementItems_GameRegionId",
                table: "AdvertisementItems",
                column: "GameRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementItems_GenreId",
                table: "AdvertisementItems",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementItems_Regions_ConsoleRegionId",
                table: "AdvertisementItems",
                column: "ConsoleRegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementItems_Regions_GameRegionId",
                table: "AdvertisementItems",
                column: "GameRegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementItems_Genres_GenreId",
                table: "AdvertisementItems",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementItems_Regions_ConsoleRegionId",
                table: "AdvertisementItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementItems_Regions_GameRegionId",
                table: "AdvertisementItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementItems_Genres_GenreId",
                table: "AdvertisementItems");

            migrationBuilder.DropIndex(
                name: "IX_AdvertisementItems_ConsoleRegionId",
                table: "AdvertisementItems");

            migrationBuilder.DropIndex(
                name: "IX_AdvertisementItems_GameRegionId",
                table: "AdvertisementItems");

            migrationBuilder.DropIndex(
                name: "IX_AdvertisementItems_GenreId",
                table: "AdvertisementItems");

            migrationBuilder.DropColumn(
                name: "ConsoleRegionId",
                table: "AdvertisementItems");

            migrationBuilder.DropColumn(
                name: "DateDeveloped",
                table: "AdvertisementItems");

            migrationBuilder.DropColumn(
                name: "Developer",
                table: "AdvertisementItems");

            migrationBuilder.DropColumn(
                name: "GameRegionId",
                table: "AdvertisementItems");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "AdvertisementItems");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AdvertisementItems");
        }
    }
}
