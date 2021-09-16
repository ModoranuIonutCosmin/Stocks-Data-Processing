using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class RenamedColumnTransactionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvestedSum",
                table: "Transactions",
                newName: "InvestedAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvestedAmount",
                table: "Transactions",
                newName: "InvestedSum");
        }
    }
}
