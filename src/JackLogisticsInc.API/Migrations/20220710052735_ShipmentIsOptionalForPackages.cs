using Microsoft.EntityFrameworkCore.Migrations;

namespace JackLogisticsInc.API.Migrations
{
    public partial class ShipmentIsOptionalForPackages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ShipmentId",
                schema: "logistics",
                table: "Packages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ShipmentId",
                schema: "logistics",
                table: "Packages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
