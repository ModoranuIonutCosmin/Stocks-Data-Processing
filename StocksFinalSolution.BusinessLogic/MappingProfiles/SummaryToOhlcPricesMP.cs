using AutoMapper;
using Stocks.General.Models.StocksInfoAggregates;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.MappingProfiles;

public class SummaryToOhlcPricesMP : Profile
{
    public SummaryToOhlcPricesMP()
    {
        CreateMap<StocksOhlc, OhlcPriceValue>();
    }
}