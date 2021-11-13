using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class addedSummariesTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StocksOHLC_Companies_CompanyTicker",
                table: "StocksOHLC");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StocksOHLC",
                table: "StocksOHLC");

            migrationBuilder.RenameTable(
                name: "StocksOHLC",
                newName: "Summaries");

            migrationBuilder.RenameIndex(
                name: "IX_StocksOHLC_CompanyTicker",
                table: "Summaries",
                newName: "IX_Summaries_CompanyTicker");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyTicker",
                table: "Summaries",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Summaries_Date_Period_CompanyTicker",
                table: "Summaries",
                columns: new[] { "Date", "Period", "CompanyTicker" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Summaries",
                table: "Summaries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Summaries_Companies_CompanyTicker",
                table: "Summaries",
                column: "CompanyTicker",
                principalTable: "Companies",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Summaries_Companies_CompanyTicker",
                table: "Summaries");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Summaries_Date_Period_CompanyTicker",
                table: "Summaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Summaries",
                table: "Summaries");

            migrationBuilder.RenameTable(
                name: "Summaries",
                newName: "StocksOHLC");

            migrationBuilder.RenameIndex(
                name: "IX_Summaries_CompanyTicker",
                table: "StocksOHLC",
                newName: "IX_StocksOHLC_CompanyTicker");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyTicker",
                table: "StocksOHLC",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StocksOHLC",
                table: "StocksOHLC",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StocksOHLC_Companies_CompanyTicker",
                table: "StocksOHLC",
                column: "CompanyTicker",
                principalTable: "Companies",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
