using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcurementPlanner.Migrations
{
    /// <inheritdoc />
    public partial class ItemItemcomponentCascadeSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemComponents_Components_ComponentId",
                table: "ItemComponents");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemComponents_Components_ComponentId",
                table: "ItemComponents",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemComponents_Components_ComponentId",
                table: "ItemComponents");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemComponents_Components_ComponentId",
                table: "ItemComponents",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
