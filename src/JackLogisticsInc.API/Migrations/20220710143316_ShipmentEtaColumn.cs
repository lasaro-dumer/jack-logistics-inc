using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JackLogisticsInc.API.Migrations
{
    public partial class ShipmentEtaColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedTimeOfArrival",
                schema: "logistics",
                table: "Shipments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedTimeOfArrival",
                schema: "logistics",
                table: "Shipments");
        }
    }
}
