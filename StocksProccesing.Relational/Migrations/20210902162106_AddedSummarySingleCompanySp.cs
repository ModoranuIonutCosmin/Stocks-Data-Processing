using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class AddedSummarySingleCompanySp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StocksDailySummaryModel");

			migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE dbo.spGetDailyStockSummarySingleCompany
						-- Add the parameters for the stored procedure here
						@fromDate DATETIMEOFFSET(7),
						@ticker nvarchar(10)
					AS
					BEGIN
						-- SET NOCOUNT ON added to prevent extra result sets from
						-- interfering with SELECT statements.
						SET NOCOUNT ON;
					
						DECLARE @LAST FLOAT;
						DECLARE @FIRST FLOAT;
					
						SELECT 
							@FIRST = t.Price
							FROM (SELECT Price, ROW_NUMBER() OVER(ORDER BY Date ASC) AS row_number_ASC 
									FROM PricesData
									WHERE DATE >= @fromDate AND CompanyTicker = @ticker
										AND Prediction = 0 
								  ) AS t
						WHERE row_number_ASC = 1;
					
					
						SELECT 
							@LAST = t.Price
							FROM (SELECT Price, ROW_NUMBER() OVER(ORDER BY Date DESC) AS row_number_DESC
									FROM PricesData
									WHERE DATE >= @fromDate AND CompanyTicker = @ticker
										AND Prediction = 0 
								  ) AS t
						WHERE row_number_DESC = 1;
					
					
						SELECT Ticker
							,UrlLogo
							,Name
							,High
							,Low
							,@FIRST as OpenValue
							,@LAST as CloseValue
						FROM (
							SELECT p.CompanyTicker AS Ticker,
							c.UrlLogo,
							Name,
							MAX(Price) AS High,
							MIN(Price) AS Low
							FROM PricesData p
								JOIN Companies c ON p.CompanyTicker = c.Ticker
							WHERE DATE >= @fromDate AND p.CompanyTicker = @ticker
								AND Prediction = 0 GROUP BY CompanyTicker, UrlLogo, Name
							)as t
					END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StocksDailySummaryModel",
                columns: table => new
                {
                    CloseValue = table.Column<double>(type: "float", nullable: false),
                    High = table.Column<double>(type: "float", nullable: false),
                    Low = table.Column<double>(type: "float", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenValue = table.Column<double>(type: "float", nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlLogo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });
        }
    }
}
