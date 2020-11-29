using Microsoft.EntityFrameworkCore.Migrations;

namespace Games4TradeAPI.Data.Migrations
{
    public partial class moveddescriptiontoaditem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Advertisements");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Announcements",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AdvertisementItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AdvertisementItems");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Advertisements",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
