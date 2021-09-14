using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class AddedUniquenessStamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueActionStamp",
                table: "Transactions",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UniqueActionStamp",
                table: "Transactions",
                column: "UniqueActionStamp",
                unique: true,
                filter: "[UniqueActionStamp] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_UniqueActionStamp",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UniqueActionStamp",
                table: "Transactions");
        }
    }
}
