using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Ticker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Ticker);
                });

            migrationBuilder.CreateTable(
                name: "PricesData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Prediction = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    StocksDataTicker = table.Column<string>(type: "nvarchar(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricesData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricesData_Companies_StocksDataTicker",
                        column: x => x.StocksDataTicker,
                        principalTable: "Companies",
                        principalColumn: "Ticker",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Ticker",
                table: "Companies",
                column: "Ticker",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PricesData_Date",
                table: "PricesData",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_PricesData_Prediction",
                table: "PricesData",
                column: "Prediction");

            migrationBuilder.CreateIndex(
                name: "IX_PricesData_StocksDataTicker",
                table: "PricesData",
                column: "StocksDataTicker");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricesData");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
