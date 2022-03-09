using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Stocks.General.ExtensionMethods;
using Stocks.General.Models;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Models;

namespace StocksFinalSolution.BusinessLogic.Features.Stocks;

public class StocksService : IStocksService
{
    private readonly ICompaniesRepository _companiesRepository;
    private readonly IStockPricesRepository _stockPricesRepository;
    private readonly IStockSummariesRepository _summariesRepository;
    private readonly IStocksTrendCalculator _stocksTrendCalculator;
    private readonly IStockMarketDisplayPriceCalculator _stockPriceCalculator;
    private readonly IMapper _mapper;

    public StocksService(ICompaniesRepository companiesRepository,
        IStockPricesRepository stockPricesRepository,
        IStockSummariesRepository summariesRepository,
        IStocksTrendCalculator stocksTrendCalculator,
        IStockMarketDisplayPriceCalculator stockPriceCalculator,
        IMapper mapper)
    {
        _companiesRepository = companiesRepository;
        _stockPricesRepository = stockPricesRepository;
        _summariesRepository = summariesRepository;
        _stocksTrendCalculator = stocksTrendCalculator;
        _stockPriceCalculator = stockPriceCalculator;
        _mapper = mapper;
    }
    
    public async Task<StockReportSingle> GetReportForSingleCompany(string ticker)
    {
        var lastSummaryEntry = await _summariesRepository
            .GetLastSummaryEntryForTickerAndInterval(ticker, TimeSpan.FromDays(7));

        var companyData = _companiesRepository.GetCompanyData(ticker);

        return MergeCompanyDataWithStockSummary(companyData, lastSummaryEntry);
    }
    
    public async Task<List<StockReportSingle>> GetReportForOfAllCompanies()
    {
        var companies = await _companiesRepository.GetAllAsync();
        var summaries = await _summariesRepository.GetLastSummaryEntryForAll(TimeSpan.FromDays(1));

        var result = companies.Join(summaries, c => c.Ticker,
            s => s.CompanyTicker,
            (company, summary) =>
            {
                return MergeCompanyDataWithStockSummary(company, summary);
            }).ToList();
        
        return result;
    }
    
    

    public async Task<StocksSummary> GetHistoricalData(string ticker,
        string interval)
    {
        Company company = _companiesRepository.GetCompanyData(ticker);
        long intervalTicks = TimespanParser.ParseTimeSpanTicks(interval);

        List<OhlcPriceValue> dataPoints = 
            _mapper.Map<List<StocksOhlc>, List<OhlcPriceValue>>(
                    await _summariesRepository.GetAllByTickerAndPeriod(ticker,
                        TimeSpan.FromTicks(intervalTicks))
                                                                )
                .OrderBy(e => e.Date)
                .ToList();

        var trend = dataPoints.Count == 0
            ? 0m
            : _stocksTrendCalculator.CalculateTrendFromOHLC(dataPoints[^1]);
        var currentBasePrice = _stockPricesRepository.GetCurrentUnitPriceByStocksCompanyTicker(ticker);

        return new StocksSummary()
        {
            Period = intervalTicks,
            UrlLogo = company.UrlLogo,
            Name = company.Name,
            Description = company.Description,
            Ticker = ticker,
            Trend = trend,
            SellPrice = _stockPriceCalculator.CalculateSellPrice(currentBasePrice),
            BuyPrice = _stockPriceCalculator.CalculateBuyPrice(currentBasePrice, 1),
            Timepoints = dataPoints
        };
    }

    private StockReportSingle MergeCompanyDataWithStockSummary(Company company, StocksOhlc summary)
    {
        var trend = _stocksTrendCalculator.CalculateTrendFromOHLC(summary);
        var price = summary.CloseValue;
        var sellPrice = _stockPriceCalculator.CalculateSellPrice(price);
        var buyPrice = _stockPriceCalculator.CalculateBuyPrice(price, 1);

        return new StockReportSingle()
        {
            BuyPrice = buyPrice.TruncateToDecimalPlaces(3),
            SellPrice = sellPrice.TruncateToDecimalPlaces(3),
            Trend = trend.TruncateToDecimalPlaces(3),
            Name = company.Name,
            Ticker = company.Ticker,
            Description = company.Description,
            UrlLogo = company.UrlLogo,
            Period = summary.Period,
            Timepoint = new OhlcPriceValue
            {
                Date = summary.Date,
                High = summary.High,
                Low = summary.Low,
                OpenValue = summary.OpenValue,
                CloseValue = summary.CloseValue,
            }
        };
    }
    
}