using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Games4TradeAPI.Data.Migrations
{
    public partial class consoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AdvertisementItems",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateManufactured",
                table: "AdvertisementItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AdvertisementItems");

            migrationBuilder.DropColumn(
                name: "DateManufactured",
                table: "AdvertisementItems");
        }
    }
}
