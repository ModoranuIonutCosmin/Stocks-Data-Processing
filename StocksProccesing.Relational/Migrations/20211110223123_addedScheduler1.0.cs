using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class addedScheduler10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Summaries_Companies_CompanyTicker",
                table: "Summaries");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Summaries_Date_Period_CompanyTicker",
                table: "Summaries");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyTicker",
                table: "Summaries",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_Summaries_Period",
                table: "Summaries",
                column: "Period");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_Name",
                table: "Actions",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Summaries_Companies_CompanyTicker",
                table: "Summaries",
                column: "CompanyTicker",
                principalTable: "Companies",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Summaries_Companies_CompanyTicker",
                table: "Summaries");

            migrationBuilder.DropIndex(
                name: "IX_Summaries_Period",
                table: "Summaries");

            migrationBuilder.DropIndex(
                name: "IX_Actions_Name",
                table: "Actions");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Summaries_Companies_CompanyTicker",
                table: "Summaries",
                column: "CompanyTicker",
                principalTable: "Companies",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
