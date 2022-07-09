using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JackLogisticsInc.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "logistics");

            migrationBuilder.CreateTable(
                name: "Shipments",
                schema: "logistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DestinationAddress = table.Column<string>(type: "text", nullable: true),
                    LeftForDestinationAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeliveredAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                schema: "logistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    AddressData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                schema: "logistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Building = table.Column<string>(type: "text", nullable: true),
                    Floor = table.Column<string>(type: "text", nullable: true),
                    Corridor = table.Column<string>(type: "text", nullable: true),
                    Shelf = table.Column<string>(type: "text", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    PackageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalSchema: "logistics",
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                schema: "logistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LocationId = table.Column<int>(type: "integer", nullable: true),
                    ShipmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Locations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "logistics",
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "logistics",
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "logistics",
                table: "Warehouses",
                columns: new[] { "Id", "AddressData", "Name" },
                values: new object[] { 1, "F38M+QM Glória, Estrela - RS", "Main Street Deposit" });

            migrationBuilder.InsertData(
                schema: "logistics",
                table: "Locations",
                columns: new[] { "Id", "Building", "Corridor", "Floor", "PackageId", "Shelf", "WarehouseId" },
                values: new object[,]
                {
                    { 1, "1A", "A", "1", 0, "001", 1 },
                    { 2, "1A", "A", "1", 0, "002", 1 },
                    { 3, "1A", "B", "1", 0, "001", 1 },
                    { 4, "1A", "B", "1", 0, "002", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_WarehouseId",
                schema: "logistics",
                table: "Locations",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_LocationId",
                schema: "logistics",
                table: "Packages",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ShipmentId",
                schema: "logistics",
                table: "Packages",
                column: "ShipmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Packages",
                schema: "logistics");

            migrationBuilder.DropTable(
                name: "Locations",
                schema: "logistics");

            migrationBuilder.DropTable(
                name: "Shipments",
                schema: "logistics");

            migrationBuilder.DropTable(
                name: "Warehouses",
                schema: "logistics");
        }
    }
}
