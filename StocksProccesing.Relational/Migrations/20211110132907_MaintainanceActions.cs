using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class MaintainanceActions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Actions",
                newName: "Schedule");

            migrationBuilder.RenameColumn(
                name: "ReccurencyTimeSpanTicks",
                table: "Actions",
                newName: "Interval");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Actions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Actions");

            migrationBuilder.RenameColumn(
                name: "Schedule",
                table: "Actions",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Interval",
                table: "Actions",
                newName: "ReccurencyTimeSpanTicks");
        }
    }
}
