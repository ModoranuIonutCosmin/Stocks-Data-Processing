﻿using System.Threading.Tasks;
using Stocks_Data_Processing.Models;

namespace Stocks_Data_Processing.Interfaces.Services
{
    /// <summary>
    /// Interfata cu metode ce isi propun obtinerea datelor din prezent
    /// referitoare la valoarea stock-urilor unei companii 
    /// pornind de la datele oferite de Yahoo Finance
    /// </summary>
    public interface ICurrentStockInfoYahooScraperService
    {
        string BuildResourceLink(string ticker);
        Task<StockCurrentInfoResponse> GatherAsync(string ticker);
    }
}