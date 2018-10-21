using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Games4Trade.Data.Migrations
{
    public partial class fixeddate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateManufactured",
                table: "AdvertisementItems");

            migrationBuilder.DropColumn(
                name: "DateDeveloped",
                table: "AdvertisementItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateReleased",
                table: "AdvertisementItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateReleased",
                table: "AdvertisementItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateManufactured",
                table: "AdvertisementItems",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeveloped",
                table: "AdvertisementItems",
                nullable: true);
        }
    }
}
