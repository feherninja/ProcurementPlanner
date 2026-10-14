using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcurementPlanner.Migrations
{
    /// <inheritdoc />
    public partial class ComponentChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPerUnit",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Components");

            migrationBuilder.AddColumn<int>(
                name: "OrderPackageSize",
                table: "Components",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Components",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderPackageSize",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Components");

            migrationBuilder.AddColumn<decimal>(
                name: "CostPerUnit",
                table: "Components",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Components",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
