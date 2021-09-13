using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class AddedBalanceRefiller : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentProcessor = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CurrencyTicker = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    DateFinalized = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
