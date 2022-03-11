using AutoMapper;
using Stocks.General.Models;
using Stocks.General.Models.StocksInfoAggregates;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Models;

namespace StocksFinalSolution.BusinessLogic.MappingProfiles;

public class SummaryToOhlcPricesMP : Profile
{
    public SummaryToOhlcPricesMP()
    {
        CreateMap<StocksOhlc, OhlcPriceValue>();
    }
}