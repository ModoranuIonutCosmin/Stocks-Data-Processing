using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksProccesing.Relational.Migrations
{
    public partial class AddedPeriodicalSummarySp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE [dbo].spGetPeriodicalSummary
                @StartDate DATETIMEOFFSET(7),
	            @Ticker nvarchar(10),
                @IntervalDays INT = 1,
                @EndDate DATETIMEOFFSET(7) = '01/01/2999'
            AS
            BEGIN
                SELECT
                       High,
                       Low,
                       OpenValue,
                       CloseValue,
                       DateDay as Date
                FROM
                (
                    SELECT CompanyTicker AS Ticker,
                           DATEADD(DAY, DATEDIFF(DAY, 0, Date) / @IntervalDays * @IntervalDays, 0) AT TIME ZONE 'UTC' AS DateDay,
                           MAX(Price) AS High,
                           MIN(Price) AS Low,
                           (
                               SELECT TOP 1
                                   DO.Price
                               FROM PricesData AS DO
                               WHERE DO.[Date] = MIN(A.[Date])
                                     AND DO.CompanyTicker = A.CompanyTicker
                                     AND Prediction = 0
                               ORDER BY Date,
                                        ID
                           ) As 'OpenValue',
                           (
                               SELECT TOP 1
                                   DC.Price
                               FROM PricesData AS DC
                               WHERE DC.[Date] = MAX(A.[Date])
                                     AND DC.CompanyTicker = A.CompanyTicker
                                     AND Prediction = 0
                               ORDER BY Date,
                                        ID
                           ) As 'CloseValue'
                    FROM PricesData AS A
                    WHERE [Date] >= @StartDate
                          AND [Date] <= @EndDate
                          AND Prediction = 0
			              AND CompanyTicker = @Ticker
                    GROUP BY DATEADD(DAY, DATEDIFF(DAY, 0, [Date]) / @IntervalDays * @IntervalDays, 0),
                             CompanyTicker
                ) temp
                ORDER BY Date
            END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
