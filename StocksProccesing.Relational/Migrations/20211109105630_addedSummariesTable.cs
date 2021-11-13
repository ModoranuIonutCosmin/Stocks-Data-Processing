using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class addedSummariesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReccurencyTimeSpanTicks",
                table: "Actions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "StocksOHLC",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    High = table.Column<decimal>(type: "decimal(20,4)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(20,4)", nullable: false),
                    OpenValue = table.Column<decimal>(type: "decimal(20,4)", nullable: false),
                    CloseValue = table.Column<decimal>(type: "decimal(20,4)", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Period = table.Column<long>(type: "bigint", nullable: false),
                    CompanyTicker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StocksOHLC", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StocksOHLC_Companies_CompanyTicker",
                        column: x => x.CompanyTicker,
                        principalTable: "Companies",
                        principalColumn: "Ticker",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StocksOHLC_CompanyTicker",
                table: "StocksOHLC",
                column: "CompanyTicker");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StocksOHLC");

            migrationBuilder.DropColumn(
                name: "ReccurencyTimeSpanTicks",
                table: "Actions");
        }
    }
}
