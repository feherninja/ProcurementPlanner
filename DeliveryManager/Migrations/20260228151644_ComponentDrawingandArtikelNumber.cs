using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryManager.Migrations
{
    /// <inheritdoc />
    public partial class ComponentDrawingandArtikelNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DrawingNumber",
                table: "Components",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "Components",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrawingNumber",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "Components");
        }
    }
}
