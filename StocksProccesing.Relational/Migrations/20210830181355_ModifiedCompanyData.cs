using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class ModifiedCompanyData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Companies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlLogo",
                table: "Companies",
                type: "nvarchar(350)",
                maxLength: 350,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "UrlLogo",
                table: "Companies");
        }
    }
}
