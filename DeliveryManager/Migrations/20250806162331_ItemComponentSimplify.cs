using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryManager.Migrations
{
    /// <inheritdoc />
    public partial class ItemComponentSimplify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "ItemComponents",
                type: "INTEGER",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 18,
                oldScale: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ItemComponents",
                type: "TEXT",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldPrecision: 18,
                oldScale: 3);
        }
    }
}
