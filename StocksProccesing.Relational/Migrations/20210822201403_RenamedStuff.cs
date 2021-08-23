using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class RenamedStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricesData_Companies_StocksDataTicker",
                table: "PricesData");

            migrationBuilder.DropIndex(
                name: "IX_PricesData_StocksDataTicker",
                table: "PricesData");

            migrationBuilder.DropColumn(
                name: "StocksDataTicker",
                table: "PricesData");

            migrationBuilder.RenameColumn(
                name: "Ticker",
                table: "PricesData",
                newName: "CompanyTicker");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "Companies",
                newName: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PricesData_CompanyTicker",
                table: "PricesData",
                column: "CompanyTicker");

            migrationBuilder.AddForeignKey(
                name: "FK_PricesData_Companies_CompanyTicker",
                table: "PricesData",
                column: "CompanyTicker",
                principalTable: "Companies",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricesData_Companies_CompanyTicker",
                table: "PricesData");

            migrationBuilder.DropIndex(
                name: "IX_PricesData_CompanyTicker",
                table: "PricesData");

            migrationBuilder.RenameColumn(
                name: "CompanyTicker",
                table: "PricesData",
                newName: "Ticker");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Companies",
                newName: "CompanyName");

            migrationBuilder.AddColumn<string>(
                name: "StocksDataTicker",
                table: "PricesData",
                type: "nvarchar(10)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PricesData_StocksDataTicker",
                table: "PricesData",
                column: "StocksDataTicker");

            migrationBuilder.AddForeignKey(
                name: "FK_PricesData_Companies_StocksDataTicker",
                table: "PricesData",
                column: "StocksDataTicker",
                principalTable: "Companies",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
