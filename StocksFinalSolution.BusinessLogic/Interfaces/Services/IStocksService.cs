﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Stocks.General.Models;
using StocksProcessing.API.Models;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Services;

public interface IStocksService
{
    Task<List<StockReportSingle>> GetReportForOfAllCompanies();

    Task<StocksSummary> GetHistoricalData(string ticker,
        string interval);

    Task<StockReportSingle> GetReportForSingleCompany(string ticker);
}