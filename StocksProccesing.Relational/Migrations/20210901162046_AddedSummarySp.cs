using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class AddedSummarySp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			///SummarySp===========
			migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE dbo.spGetDailyStockSummary
								-- Add the parameters for the stored procedure here
								@fromDate DATETIMEOFFSET(7)
							AS
							BEGIN
								-- SET NOCOUNT ON added to prevent extra result sets from
								-- interfering with SELECT statements.
								SET NOCOUNT ON;
							
								DECLARE @LASTS TABLE (
									Ticker NVARCHAR(10)
									,CloseValue FLOAT
									);
								DECLARE @FIRSTS TABLE (
									Ticker NVARCHAR(10)
									,OpenValue FLOAT
									);
							


								WITH list_oridinal
								AS (
									SELECT CompanyTicker
										,Price
										,ROW_NUMBER() OVER (
											PARTITION BY CompanyTicker ORDER BY DATE DESC
											) AS row_number_DESC
									FROM PricesData p
									WHERE DATE >= @fromDate
										AND Prediction = 0
									)
								INSERT INTO @LASTS
								SELECT CompanyTicker AS Ticker
									,Price AS CloseValue
								FROM list_oridinal
								WHERE row_number_DESC = 1;
							


								WITH list_oridinal
								AS (
									SELECT CompanyTicker
										,Price
										,ROW_NUMBER() OVER (
											PARTITION BY CompanyTicker ORDER BY DATE ASC
											) AS row_number_ASC
									FROM PricesData p
									WHERE DATE >= @fromDate
										AND Prediction = 0
									)
								INSERT INTO @FIRSTS
								SELECT CompanyTicker AS Ticker
									,Price AS OpenValue
								FROM list_oridinal
								WHERE row_number_ASC = 1;
							

								SELECT c1.Ticker
									,UrlLogo
									,Name
									,High
									,Low
									,OpenValue
									,CloseValue
								FROM (
									SELECT CompanyTicker AS Ticker
										,MAX(Price) AS High
										,MIN(Price) AS Low
									FROM PricesData
									WHERE DATE >= @fromDate
										AND Prediction = 0
									GROUP BY CompanyTicker
									) AS c1
								JOIN @Lasts c2 ON c1.Ticker = c2.Ticker
								JOIN @FIRSTS c3 ON c3.Ticker = c2.Ticker
								JOIN Companies c4 ON c4.Ticker = c3.Ticker;
							END");
			///====================
		}

		protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
