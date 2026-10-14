using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcurementPlanner.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyToItemAndComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Components",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CompanyId",
                table: "Items",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_CompanyId",
                table: "Components",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Companies_CompanyId",
                table: "Components",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Companies_CompanyId",
                table: "Items",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Components_Companies_CompanyId",
                table: "Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Companies_CompanyId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CompanyId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Components_CompanyId",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Components");
        }
    }
}
