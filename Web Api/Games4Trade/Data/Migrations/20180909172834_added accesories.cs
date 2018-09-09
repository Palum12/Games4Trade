using Microsoft.EntityFrameworkCore.Migrations;

namespace Games4Trade.Data.Migrations
{
    public partial class addedaccesories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessoryManufacturer",
                table: "AdvertisementItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccessoryModel",
                table: "AdvertisementItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessoryManufacturer",
                table: "AdvertisementItems");

            migrationBuilder.DropColumn(
                name: "AccessoryModel",
                table: "AdvertisementItems");
        }
    }
}
