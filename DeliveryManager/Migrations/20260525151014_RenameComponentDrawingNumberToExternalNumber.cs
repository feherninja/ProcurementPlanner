using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryManager.Migrations
{
    /// <inheritdoc />
    public partial class RenameComponentDrawingNumberToExternalNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DrawingNumber",
                table: "Components",
                newName: "ExternalNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExternalNumber",
                table: "Components",
                newName: "DrawingNumber");
        }
    }
}
