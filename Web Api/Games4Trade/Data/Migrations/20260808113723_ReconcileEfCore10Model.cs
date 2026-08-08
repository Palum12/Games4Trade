using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Games4TradeAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReconcileEfCore10Model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Historical migrations already create the derived item properties in
            // AdvertisementItems. Their old snapshot incorrectly described three
            // separate tables. This baseline migration updates the EF Core 10
            // snapshot without attempting destructive or duplicate DDL.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This migration is a model-snapshot baseline and intentionally has no DDL.
        }
    }
}
